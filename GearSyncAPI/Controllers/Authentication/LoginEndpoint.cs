using Dapper;
using FastEndpoints;
using Microsoft.Data.Sqlite;

namespace GearSyncAPI.Controllers.Authentication;

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    public override void Configure()
    {
        Post("/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        // Validate the request
        if (string.IsNullOrWhiteSpace(req.DealerID) || string.IsNullOrWhiteSpace(req.Password))
        {
            await SendAsync(new LoginResponse { Token = null, Error = "DealerID and Password are required." }, statusCode: 400, cancellation: ct);
            return;
        }

        await using var connection = new SqliteConnection("Data Source=GearSync.db");
        await connection.OpenAsync(ct);

        // Check for an active session
        const string sessionCheckQuery = "SELECT ExpiresAt FROM Sessions WHERE DealerID = @DealerID";
        var existingSession = await connection.QuerySingleOrDefaultAsync<DateTime?>(sessionCheckQuery, new { DealerID = req.DealerID });

        if (existingSession.HasValue && existingSession.Value > DateTime.UtcNow)
        {
            // Deny login if an active session exists
            await SendAsync(new LoginResponse
            {
                Token = null,
                Error = "Another session is already active. Please wait until it expires or contact support."
            }, statusCode: 403, cancellation: ct);
            return;
        }

        // Retrieve dealer's password hash
        const string dealerQuery = "SELECT PasswordHash FROM Dealers WHERE DealerID = @DealerID";
        var passwordHash = await connection.QuerySingleOrDefaultAsync<string>(dealerQuery, new { DealerID = req.DealerID });

        if (passwordHash == null)
        {
            await SendAsync(new LoginResponse { Token = null, Error = "Invalid DealerID or Password." }, statusCode: 401, cancellation: ct);
            return;
        }

        // Verify the password
        if (!BCrypt.Net.BCrypt.Verify(req.Password, passwordHash))
        {
            await SendAsync(new LoginResponse { Token = null, Error = "Invalid DealerID or Password." }, statusCode: 401, cancellation: ct);
            return;
        }

        // Generate a JWT token
        var token = GenerateToken(req.DealerID);
        var expiration = DateTime.UtcNow.AddHours(1); // Token expires in 1 hour

        // Insert or update session in the Sessions table
        const string sessionQuery = @"
            INSERT INTO Sessions (DealerID, JWTToken, ExpiresAt)
            VALUES (@DealerID, @JWTToken, @ExpiresAt)
            ON CONFLICT(DealerID) DO UPDATE SET
                JWTToken = @JWTToken,
                ExpiresAt = @ExpiresAt";
        await connection.ExecuteAsync(sessionQuery, new
        {
            DealerID = req.DealerID,
            JWTToken = token,
            ExpiresAt = expiration
        });

        // Send the response
        await SendOkAsync(new LoginResponse { Token = token, ExpiresAt = expiration });
    }

    private string GenerateToken(string dealerID)
    {
        // Example token generation (replace with real JWT implementation)
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{dealerID}-{Guid.NewGuid()}"));
    }
}

public class LoginRequest
{
    public string DealerID { get; set; }
    public string Password { get; set; }
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string Error { get; set; } = string.Empty;
}
