using Dapper;
using FastEndpoints;
using Microsoft.Data.Sqlite;

namespace GearSyncAPI.Controllers.Cars;

public class CarFilterEndpoint : Endpoint<CarFilterRequest, CarFilterResponse>
{
    public override void Configure()
    {
        Get("/cars/filter");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CarFilterRequest req, CancellationToken ct)
    {
        await using var connection = new SqliteConnection("Data Source=GearSync.db");
        await connection.OpenAsync(ct);

        // Base query
        var query = new List<string> { "SELECT * FROM Cars WHERE 1=1" };

        // Build dynamic WHERE conditions based on query parameters
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(req.Make))
        {
            query.Add("AND Make = @Make");
            parameters.Add("Make", req.Make);
        }

        if (!string.IsNullOrWhiteSpace(req.Model))
        {
            query.Add("AND Model = @Model");
            parameters.Add("Model", req.Model);
        }

        if (req.Year != null)
        {
            query.Add("AND Year = @Year");
            parameters.Add("Year", req.Year);
        }

        if (!string.IsNullOrWhiteSpace(req.Colours))
        {
            query.Add("AND Colours LIKE @Colours");
            parameters.Add("Colours", $"%{req.Colours}%");
        }

        if (!string.IsNullOrWhiteSpace(req.Body))
        {
            query.Add("AND Body = @Body");
            parameters.Add("Body", req.Body);
        }

        if (!string.IsNullOrWhiteSpace(req.Transmission))
        {
            query.Add("AND Transmission = @Transmission");
            parameters.Add("Transmission", req.Transmission);
        }

        if (!string.IsNullOrWhiteSpace(req.FuelType))
        {
            query.Add("AND FuelType = @FuelType");
            parameters.Add("FuelType", req.FuelType);
        }

        if (req.Seats != null)
        {
            query.Add("AND Seats = @Seats");
            parameters.Add("Seats", req.Seats);
        }

        if (req.Doors != null)
        {
            query.Add("AND Doors = @Doors");
            parameters.Add("Doors", req.Doors);
        }

        // Finalize query
        var finalQuery = string.Join(" ", query);

        // Execute the query
        var cars = await connection.QueryAsync<CarDetails>(finalQuery, parameters);

        // Respond with filtered cars
        await SendOkAsync(new CarFilterResponse { Cars = cars.ToList() });
    }
}

public class CarFilterRequest
{
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? Colours { get; set; }
    public string? Body { get; set; }
    public string? Transmission { get; set; }
    public string? FuelType { get; set; }
    public int? Seats { get; set; }
    public int? Doors { get; set; }
}

public class CarFilterResponse
{
    public List<CarDetails> Cars { get; set; } = new();
}

