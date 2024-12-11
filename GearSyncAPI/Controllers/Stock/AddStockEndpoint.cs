using Dapper;
using FastEndpoints;
using Microsoft.Data.Sqlite;

namespace GearSyncAPI.Controllers.Stock;

public class AddStockEndpoint : Endpoint<AddStockRequest, AddStockResponse>
{
    public override void Configure()
    {
        Post("/stock/add");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AddStockRequest req, CancellationToken ct)
    {
        // Validate the request
        var validationError = ValidateRequest(req);
        if (!string.IsNullOrEmpty(validationError))
        {
            await SendAsync(new AddStockResponse { Success = false, Error = validationError }, statusCode: 400, cancellation: ct);
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
            await SendAsync(new AddStockResponse { Success = false, Error = "Invalid dealer credentials or session expired." }, statusCode: 401, cancellation: ct);
            return;
        }

        // Check if car exists and validate color
        const string carValidationQuery = @"
            SELECT Colours
            FROM Cars
            WHERE CarID = @CarID";
        var carColours = await connection.QuerySingleOrDefaultAsync<string>(carValidationQuery, new { req.CarID });

        if (string.IsNullOrWhiteSpace(carColours))
        {
            await SendAsync(new AddStockResponse { Success = false, Error = "Car not found." }, statusCode: 404, cancellation: ct);
            return;
        }

        var availableColours = carColours.Split(',').Select(c => c.Trim()).ToList();
        if (!availableColours.Contains(req.Colour, StringComparer.OrdinalIgnoreCase))
        {
            await SendAsync(new AddStockResponse
            {
                Success = false,
                Error = $"Invalid colour '{req.Colour}' for the selected car. Available colours: {string.Join(", ", availableColours)}."
            }, statusCode: 400, cancellation: ct);
            return;
        }

        // Update or Insert stock
        const string upsertStockQuery = @"
            INSERT INTO Stock (CarID, DealerID, StockLevel, Colour)
            VALUES (@CarID, @DealerID, @StockLevel, @Colour)
            ON CONFLICT(CarID, DealerID, Colour) DO UPDATE 
            SET StockLevel = StockLevel + @StockLevel";
        await connection.ExecuteAsync(upsertStockQuery, new { req.CarID, req.DealerID, req.StockLevel, req.Colour });

        await SendOkAsync(new AddStockResponse { Success = true });
    }

    private string ValidateRequest(AddStockRequest req)
    {
        if (req.CarID <= 0) return "Invalid CarID.";
        if (req.DealerID <= 0) return "Invalid DealerID.";
        if (string.IsNullOrWhiteSpace(req.Token)) return "Token is required.";
        if (string.IsNullOrWhiteSpace(req.Colour)) return "Colour is required.";
        if (req.StockLevel <= 0) return "Stock level must be greater than 0.";

        return string.Empty; // No validation errors
    }
}

public class AddStockRequest
{
    public int CarID { get; set; }
    public int DealerID { get; set; }
    public string Token { get; set; } = string.Empty;
    public int StockLevel { get; set; }
    public string Colour { get; set; } = string.Empty;
}

public class AddStockResponse
{
    public bool Success { get; set; }
    public string Error { get; set; } = string.Empty;
}
