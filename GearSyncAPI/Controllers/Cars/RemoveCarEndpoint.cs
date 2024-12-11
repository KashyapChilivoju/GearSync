using Dapper;
using FastEndpoints;
using Microsoft.Data.Sqlite;

namespace GearSyncAPI.Controllers.Cars;

public class RemoveCarEndpoint : Endpoint<RemoveCarRequest, RemoveCarResponse>
{
    public override void Configure()
    {
        Delete("/cars/remove");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RemoveCarRequest req, CancellationToken ct)
    {
        // Validate the request
        if (req.CarID <= 0)
        {
            await SendAsync(new RemoveCarResponse { Success = false, Error = "Invalid CarID." }, statusCode: 400, cancellation: ct);
            return;
        }

        await using var connection = new SqliteConnection("Data Source=GearSync.db");
        await connection.OpenAsync(ct);

        // Check if the car exists
        const string checkCarQuery = "SELECT COUNT(1) FROM Cars WHERE CarID = @CarID";
        var carExists = await connection.QuerySingleAsync<int>(checkCarQuery, new { req.CarID }) > 0;

        if (!carExists)
        {
            await SendAsync(new RemoveCarResponse { Success = false, Error = "Car not found." }, statusCode: 404, cancellation: ct);
            return;
        }

        // Check if the car is in stock
        const string checkStockQuery = @"
            SELECT Dealers.DealerName
            FROM Stock
            INNER JOIN Dealers ON Stock.DealerID = Dealers.DealerID
            WHERE Stock.CarID = @CarID";
        var dealersWithStock = await connection.QueryAsync<string>(checkStockQuery, new { req.CarID });

        if (dealersWithStock.Any())
        {
            await SendAsync(new RemoveCarResponse
            {
                Success = false,
                Error = $"Car is in stock for the following dealers: {string.Join(", ", dealersWithStock)}. Please contact support for further assistance."
            }, statusCode: 409, cancellation: ct);
            return;
        }

        // Remove the car from the database
        const string deleteCarQuery = "DELETE FROM Cars WHERE CarID = @CarID";
        await connection.ExecuteAsync(deleteCarQuery, new { req.CarID });

        await SendOkAsync(new RemoveCarResponse { Success = true });
    }
}

public class RemoveCarRequest
{
    public int CarID { get; set; }
}

public class RemoveCarResponse
{
    public bool Success { get; set; }
    public string Error { get; set; } = string.Empty;
}
