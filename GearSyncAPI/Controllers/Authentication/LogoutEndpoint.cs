using Dapper;
using FastEndpoints;
using Microsoft.Data.Sqlite;

namespace GearSyncAPI.Controllers.Authentication;

public class LogoutEndpoint : Endpoint<LogoutRequest, LogoutResponse>
{
    public override void Configure()
    {
        Delete("/logout");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LogoutRequest req, CancellationToken ct)
    {
        // Validate DealerID and Token
        if (string.IsNullOrWhiteSpace(req.DealerID) || string.IsNullOrWhiteSpace(req.Token))
        {
            await SendAsync(new LogoutResponse
            {
                Success = false,
                Error = "DealerID and Token are required."
            }, statusCode: 400, cancellation: ct);
            return;
        }

        await using var connection = new SqliteConnection("Data Source=GearSync.db");
        await connection.OpenAsync(ct);

        // Check if the provided token exists and matches the dealer
        const string sessionCheckQuery = "SELECT 1 FROM Sessions WHERE DealerID = @DealerID AND JWTToken = @Token";
        var sessionExists = await connection.QuerySingleOrDefaultAsync<int?>(sessionCheckQuery, new { DealerID = req.DealerID, Token = req.Token });

        if (sessionExists == null)
        {
            await SendAsync(new LogoutResponse
            {
                Success = false,
                Error = "Invalid token or session does not exist."
            }, statusCode: 401, cancellation: ct);
            return;
        }

        // Delete the session for the specific DealerID and Token
        const string deleteSessionQuery = "DELETE FROM Sessions WHERE DealerID = @DealerID AND JWTToken = @Token";
        var rowsAffected = await connection.ExecuteAsync(deleteSessionQuery, new { DealerID = req.DealerID, Token = req.Token });

        if (rowsAffected == 0)
        {
            // If no session was found for the DealerID and Token
            await SendAsync(new LogoutResponse
            {
                Success = false,
                Error = "No active session found for the provided DealerID and Token."
            }, statusCode: 404, cancellation: ct);
        }
        else
        {
            // Successfully deleted the session
            await SendOkAsync(new LogoutResponse
            {
                Success = true,
                Message = "Logout successful. Session has been terminated."
            });
        }
    }
}

public class LogoutRequest
{
    public string DealerID { get; set; }
    public string Token { get; set; }
}

public class LogoutResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
}
