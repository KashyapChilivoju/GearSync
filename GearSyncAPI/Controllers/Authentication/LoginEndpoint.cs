using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using FastEndpoints;
using Microsoft.Data.Sqlite;
using Microsoft.IdentityModel.Tokens;

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
        using var connection = new SqliteConnection("Data Source=gearsync.db");
        connection.Open();

        // Check dealer credentials
        var query = "SELECT DealerID, PasswordHash FROM Dealers WHERE DealerID = @DealerID";
        var dealer = await connection.QueryFirstOrDefaultAsync(query, new { req.DealerID });

        if (dealer == null || !BCrypt.Net.BCrypt.Verify(req.Password, dealer?.DealerPasswordHash))
        {
            await SendErrorsAsync(statusCode: 401, cancellation: ct);
            return;
        }

        // Invalidate existing session
        query = "DELETE FROM Sessions WHERE DealerID = @DealerID";
        await connection.ExecuteAsync(query, new { DealerID = dealer.DealerID });

        // Generate a new token
        var token = GenerateJwtToken(dealer.DealerID);
        var expiry = DateTime.UtcNow.AddHours(1);

        // Create a new session
        query = "INSERT INTO Sessions (DealerID, JWTToken, ExpiresAt) VALUES (@DealerID, @Token, @ExpiresAt)";
        await connection.ExecuteAsync(query, new { DealerID = dealer.DealerID, Token = token, ExpiresAt = expiry });

        // Return the token
        await SendOkAsync(new LoginResponse { Token = token }, ct);
    }

    private string GenerateJwtToken(int dealerId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKey"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: new[] { new Claim("DealerID", dealerId.ToString()) },
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginRequest
{
    public int DealerID { get; set; }
    public string Password { get; set; }
}

public class LoginResponse
{
    public string Token { get; set; }
}