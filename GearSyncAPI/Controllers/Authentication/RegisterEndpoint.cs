using Dapper;
using FastEndpoints;
using Microsoft.Data.Sqlite;

namespace GearSyncAPI.Controllers.Authentication;

public class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
{
    public override void Configure()
    {
        Post("/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        // Validate the request
        if (string.IsNullOrWhiteSpace(req.DealerName) || string.IsNullOrWhiteSpace(req.Password))
        {
            await SendAsync(new RegisterResponse() { Error = "DealerName and Password are required." }, statusCode: 400, cancellation: ct);
            return;
        }

        await using var connection = new SqliteConnection("Data Source=GearSync.db");
        connection.Open();

        // Hash the password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);

        // Insert the dealer into the database
        const string query = "INSERT INTO Dealers (DealerName, PasswordHash) VALUES (@DealerName, @PasswordHash)";
        try
        {
            await connection.ExecuteAsync(query, new { DealerName = req.DealerName, PasswordHash = passwordHash });
        }
        catch (SqliteException ex) when (ex.SqliteErrorCode == 19) // Unique constraint violation
        {
            await SendAsync(new RegisterResponse { Error = "DealerName already exists." }, statusCode: 400, cancellation: ct);
            return;
        }
        // Get the DealerID of the newly created dealer
        var dealerID = await connection.QuerySingleAsync<int>("SELECT last_insert_rowid()");

        // Return the DealerID to the client
        await SendOkAsync(new RegisterResponse { DealerID = dealerID });
    }
}

public class RegisterRequest
{
    public string DealerName { get; set; }
    public string Password { get; set; }
}

public class RegisterResponse
{
    public int? DealerID { get; set; }
    public string Error { get; set; }
}