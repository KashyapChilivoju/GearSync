using Dapper;
using FastEndpoints;
using Microsoft.Data.Sqlite;

namespace GearSyncAPI.Controllers.Stock;

public class RemoveStockEndpoint : Endpoint<RemoveStockRequest, RemoveStockResponse>
{
    public override void Configure()
    {
        Delete("/stock/remove");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RemoveStockRequest req, CancellationToken ct)
    {
        // Validate the request
        var validationError = ValidateRequest(req);
        if (!string.IsNullOrEmpty(validationError))
        {
            await SendAsync(new RemoveStockResponse { Success = false, Error = validationError }, statusCode: 400, cancellation: ct);
            return;
        }

        await using var connection = new SqliteConnection("Data Source=GearSync.db");
        await connection.OpenAsync(ct);

        // Validate dealer with token
        const string dealerValidationQuery = @"
            SELECT COUNT(1)
            FROM Sessions
            WHERE DealerID = @DealerID AND JWTToken = @Token AND ExpiresAt > datetime('now')";
        var isDealerValid = await connection.QuerySingleAsync<int>(dealerValidationQuery, new { req.DealerID, req.Token }) > 0;

        if (!isDealerValid)
        {
            await SendAsync(new RemoveStockResponse { Success = false, Error = "Invalid dealer credentials or session expired." }, statusCode: 401, cancellation: ct);
            return;
        }

        // Check if stock exists
        const string stockValidationQuery = @"
            SELECT StockLevel
            FROM Stock
            WHERE CarID = @CarID AND DealerID = @DealerID";
        var stockLevel = await connection.QuerySingleOrDefaultAsync<int?>(stockValidationQuery, new { req.CarID, req.DealerID });

        if (stockLevel == null)
        {
            await SendAsync(new RemoveStockResponse { Success = false, Error = "Stock entry not found." }, statusCode: 404, cancellation: ct);
            return;
        }
        
        // Remove stock entry completely
        const string deleteStockQuery = "DELETE FROM Stock WHERE CarID = @CarID AND DealerID = @DealerID";
        await connection.ExecuteAsync(deleteStockQuery, new { req.CarID, req.DealerID });

        await SendOkAsync(new RemoveStockResponse { Success = true });
    }

    private string ValidateRequest(RemoveStockRequest req)
    {
        if (req.CarID <= 0) return "Invalid CarID.";
        if (req.DealerID <= 0) return "Invalid DealerID.";
        if (string.IsNullOrWhiteSpace(req.Token)) return "Token is required.";
        
        return string.Empty; // No validation errors
    }
}

public class RemoveStockRequest
{
    public int CarID { get; set; }
    public int DealerID { get; set; }
    public string Token { get; set; } = string.Empty;
}

public class RemoveStockResponse
{
    public bool Success { get; set; }
    public string Error { get; set; } = string.Empty;
}
