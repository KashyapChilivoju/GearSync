using Dapper;
using FastEndpoints;
using Microsoft.Data.Sqlite;

namespace GearSyncAPI.Controllers.Stock;

public class UpdateStockEndpoint : Endpoint<UpdateStockRequest, UpdateStockResponse>
{
    public override void Configure()
    {
        Put("/stock/update");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateStockRequest req, CancellationToken ct)
    {
        // Validate the request
        var validationError = ValidateRequest(req);
        if (!string.IsNullOrEmpty(validationError))
        {
            await SendAsync(new UpdateStockResponse { Success = false, Error = validationError }, statusCode: 400, cancellation: ct);
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
            await SendAsync(new UpdateStockResponse { Success = false, Error = "Invalid dealer credentials or session expired." }, statusCode: 401, cancellation: ct);
            return;
        }

        // Check if stock entry exists
        const string stockValidationQuery = @"
            SELECT StockLevel
            FROM Stock
            WHERE CarID = @CarID AND DealerID = @DealerID AND Colour = @Colour";
        var stockLevel = await connection.QuerySingleOrDefaultAsync<int?>(stockValidationQuery, new { req.CarID, req.DealerID, req.Colour });

        if (stockLevel == null)
        {
            await SendAsync(new UpdateStockResponse { Success = false, Error = "Stock entry not found for the specified car and colour. Please add to Stock first." }, statusCode: 404, cancellation: ct);
            return;
        }

        // Update stock level
        const string updateStockQuery = @"
            UPDATE Stock
            SET StockLevel = @StockLevel
            WHERE CarID = @CarID AND DealerID = @DealerID AND Colour = @Colour";
        await connection.ExecuteAsync(updateStockQuery, new { req.StockLevel, req.CarID, req.DealerID, req.Colour });

        await SendOkAsync(new UpdateStockResponse { Success = true });
    }

    private string ValidateRequest(UpdateStockRequest req)
    {
        if (req.CarID <= 0) return "Invalid CarID.";
        if (req.DealerID <= 0) return "Invalid DealerID.";
        if (string.IsNullOrWhiteSpace(req.Token)) return "Token is required.";
        if (string.IsNullOrWhiteSpace(req.Colour)) return "Colour is required.";
        if (req.StockLevel < 0) return "Stock level cannot be negative.";

        return string.Empty; // No validation errors
    }
}

public class UpdateStockRequest
{
    public int CarID { get; set; }
    public int DealerID { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Colour { get; set; } = string.Empty;
    public int StockLevel { get; set; }
}

public class UpdateStockResponse
{
    public bool Success { get; set; }
    public string Error { get; set; } = string.Empty;
}
