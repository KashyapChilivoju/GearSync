using Dapper;
using FastEndpoints;
using Microsoft.Data.Sqlite;

namespace GearSyncAPI.Controllers.Stock;

public class GetDealerStockEndpoint : Endpoint<GetDealerStockRequest, GetDealerStockResponse>
{
    public override void Configure()
    {
        Post("/stock/dealer");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetDealerStockRequest req, CancellationToken ct)
    {
        // Validate the request
        if (req.DealerID <= 0 || string.IsNullOrWhiteSpace(req.Token))
        {
            await SendAsync(new GetDealerStockResponse { Success = false, Error = "DealerID and Token are required." }, statusCode: 400, cancellation: ct);
            return;
        }

        await using var connection = new SqliteConnection("Data Source=GearSync.db");
        await connection.OpenAsync(ct);

        // Validate dealer credentials
        const string dealerValidationQuery = @"
            SELECT COUNT(1)
            FROM Sessions
            WHERE DealerID = @DealerID AND JWTToken = @Token AND ExpiresAt > datetime('now')";
        var isDealerValid = await connection.QuerySingleAsync<int>(dealerValidationQuery, new { req.DealerID, req.Token }) > 0;

        if (!isDealerValid)
        {
            await SendAsync(new GetDealerStockResponse { Success = false, Error = "Invalid dealer credentials or session expired." }, statusCode: 401, cancellation: ct);
            return;
        }

        // Fetch dealer's stock
        const string stockQuery = @"
            SELECT 
                s.CarID,
                c.Make,
                c.Model,
                c.Year,
                s.Colour, -- Specific colour in stock
                c.Body,
                c.Transmission,
                c.FuelType,
                c.Seats,
                c.Doors,
                s.StockLevel
            FROM Stock s
            INNER JOIN Cars c ON s.CarID = c.CarID
            WHERE s.DealerID = @DealerID";
        var stock = await connection.QueryAsync<DealerStockItem>(stockQuery, new { req.DealerID });

        await SendOkAsync(new GetDealerStockResponse { Success = true, Stock = stock.ToList() });
    }
}

public class GetDealerStockRequest
{
    public int DealerID { get; set; }
    public string Token { get; set; } = string.Empty;
}

public class GetDealerStockResponse
{
    public bool Success { get; set; }
    public string Error { get; set; } = string.Empty;
    public List<DealerStockItem> Stock { get; set; } = new();
}

public class DealerStockItem
{
    public int CarID { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Colour { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Transmission { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public int Seats { get; set; }
    public int Doors { get; set; }
    public int StockLevel { get; set; }
}
