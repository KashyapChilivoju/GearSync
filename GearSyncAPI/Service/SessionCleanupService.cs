using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class SessionCleanupService : BackgroundService
{
    private readonly ILogger<SessionCleanupService> _logger;
    private readonly string _connectionString = "Data Source=GearSync.db";

    public SessionCleanupService(ILogger<SessionCleanupService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken); // Wait for 10 minutes

            await CleanupExpiredSessionsAsync();
        }
    }

    private async Task CleanupExpiredSessionsAsync()
    {
        try
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            // Delete expired sessions
            const string deleteExpiredSessionsQuery = @"
                DELETE FROM Sessions WHERE ExpiresAt < @CurrentTime";

            var result = await connection.ExecuteAsync(deleteExpiredSessionsQuery, new
            {
                CurrentTime = DateTime.UtcNow
            });

            _logger.LogInformation($"Deleted {result} expired session(s).");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error cleaning up expired sessions: {ex.Message}");
        }
    }
}