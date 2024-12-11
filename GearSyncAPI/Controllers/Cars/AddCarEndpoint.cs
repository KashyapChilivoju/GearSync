using Dapper;
using FastEndpoints;
using Microsoft.Data.Sqlite;

namespace GearSyncAPI.Controllers.Cars;

public class AddCarEndpoint : Endpoint<AddCarRequest, AddCarResponse>
{
    public override void Configure()
    {
        Post("/cars/add");
        AllowAnonymous();
    }

    public override async Task HandleAsync(AddCarRequest req, CancellationToken ct)
    {
        // Validate the request
        var validationError = ValidateRequest(req);
        if (!string.IsNullOrEmpty(validationError))
        {
            await SendAsync(new AddCarResponse { Success = false, Error = validationError }, statusCode: 400, cancellation: ct);
            return;
        }

        await using var connection = new SqliteConnection("Data Source=GearSync.db");
        await connection.OpenAsync(ct);

        // Check for duplicate entry
        const string duplicateCheckQuery = @"
            SELECT COUNT(1)
            FROM Cars
            WHERE Make = @Make
              AND Model = @Model
              AND Year = @Year
              AND Colours = @Colours
              AND Body = @Body
              AND Transmission = @Transmission
              AND FuelType = @FuelType";
        var duplicateCount = await connection.QuerySingleAsync<int>(duplicateCheckQuery, new
        {
            req.Make,
            req.Model,
            req.Year,
            req.Colours,
            req.Body,
            req.Transmission,
            req.FuelType
        });

        if (duplicateCount > 0)
        {
            await SendAsync(new AddCarResponse { Success = false, Error = "A similar car already exists in the catalog." }, statusCode: 409, cancellation: ct);
            return;
        }
        
        // Insert the car into the database
        const string query = @"
            INSERT INTO Cars (Make, Model, Year, Colours, Body, Transmission, FuelType, Seats, Doors)
            VALUES (@Make, @Model, @Year, @Colours, @Body, @Transmission, @FuelType, @Seats, @Doors)";
        await connection.ExecuteAsync(query, new
        {
            req.Make,
            req.Model,
            req.Year,
            req.Colours,
            req.Body,
            req.Transmission,
            req.FuelType,
            req.Seats,
            req.Doors
        });

        await SendOkAsync(new AddCarResponse { Success = true });
    }

    private string ValidateRequest(AddCarRequest req)
    {
        // Check for required fields
        if (string.IsNullOrWhiteSpace(req.Make)) return "Car make is required.";
        if (string.IsNullOrWhiteSpace(req.Model)) return "Car model is required.";
        if (req.Year < 1886 || req.Year > DateTime.Now.Year + 1) return "Car year must be between 1886 and the next calendar year.";
        if (string.IsNullOrWhiteSpace(req.Colours)) return "Car colours are required.";
        if (string.IsNullOrWhiteSpace(req.Body) || !IsValidBodyType(req.Body)) return "Invalid car body type.";
        if (string.IsNullOrWhiteSpace(req.Transmission) || !IsValidTransmission(req.Transmission)) return "Invalid car transmission.";
        if (string.IsNullOrWhiteSpace(req.FuelType) || !IsValidFuelType(req.FuelType)) return "Invalid car fuel type.";
        if (req.Seats <= 0 || req.Seats > 100) return "Seats must be between 1 and 100.";
        if (req.Doors <= 0 || req.Doors > 10) return "Doors must be between 1 and 10.";

        return string.Empty; // No validation errors
    }

    private bool IsValidBodyType(string body)
    {
        var validBodyTypes = new[] { "Sedan", "Hatchback", "SUV", "Truck", "Coupe", "Convertible", "Wagon", "Van" };
        return Array.Exists(validBodyTypes, type => type.Equals(body, StringComparison.OrdinalIgnoreCase));
    }

    private bool IsValidTransmission(string transmission)
    {
        var validTransmissions = new[] { "Manual", "Automatic", "Semi-Automatic" };
        return Array.Exists(validTransmissions, type => type.Equals(transmission, StringComparison.OrdinalIgnoreCase));
    }

    private bool IsValidFuelType(string fuelType)
    {
        var validFuelTypes = new[] { "Petrol", "Diesel", "Electric", "Hybrid", "CNG", "LPG" };
        return Array.Exists(validFuelTypes, type => type.Equals(fuelType, StringComparison.OrdinalIgnoreCase));
    }
}

public class AddCarRequest
{
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Colours { get; set; }
    public string Body { get; set; }
    public string Transmission { get; set; }
    public string FuelType { get; set; }
    public int Seats { get; set; }
    public int Doors { get; set; }
}

public class AddCarResponse
{
    public bool Success { get; set; }
    public string Error { get; set; } = string.Empty;
}
