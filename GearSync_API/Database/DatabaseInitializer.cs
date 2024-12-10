using System.IO;
using Microsoft.Data.Sqlite;

namespace GearSync.Database
{
    public static class DatabaseInitializer
    {
        public static void Initialize(string databasePath)
        {
            if (!File.Exists(databasePath))
            {
                using var connection = new SqliteConnection($"Data Source={databasePath}");
                connection.Open();

                // Database Schema Setup
                string schema = File.ReadAllText("Database/schema.sql");
                using var command = connection.CreateCommand();
                command.CommandText = schema;
                command.ExecuteNonQuery();
                Console.WriteLine("Database Schema defined successfully.");
                
                // Seed data
                string data = File.ReadAllText("Database/data.sql");
                using var seedCommand = connection.CreateCommand();
                seedCommand.CommandText = data;
                seedCommand.ExecuteNonQuery();
                Console.WriteLine("Database Data seeded successfully.");
            }
            else
            {
                Console.WriteLine("Database already exists. Skipping initialization.");
            }
        }
    }
}