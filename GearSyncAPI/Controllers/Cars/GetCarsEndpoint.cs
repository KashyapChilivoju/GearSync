using Dapper;
using FastEndpoints;
using Microsoft.Data.Sqlite;

namespace GearSyncAPI.Controllers.Cars;

public class GetCarsEndpoint : EndpointWithoutRequest<GetCarsResponse>
{
    public override void Configure()
    {
        Get("/cars");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await using var connection = new SqliteConnection("Data Source=GearSync.db");
        await connection.OpenAsync(ct);

        // Query to retrieve all cars from the database
        const string query = @"
            SELECT CarID, Make, Model, Year, Colours, Body, Transmission, FuelType, Seats, Doors
            FROM Cars";
        var cars = await connection.QueryAsync<CarDetails>(query);

        // Respond with the list of cars
        await SendOkAsync(new GetCarsResponse { Cars = cars.ToList() });
    }
}

public class GetCarsResponse
{
    public List<CarDetails> Cars { get; set; } = new();
}

public class CarDetails
{
    public int CarID { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int Year { get; set; }
    public string? Colours { get; set; }
    public string? Body { get; set; }
    public string? Transmission { get; set; }
    public string? FuelType { get; set; }
    public int Seats { get; set; }
    public int Doors { get; set; }
}