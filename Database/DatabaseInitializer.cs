using System.IO;
using Microsoft.Data.Sqlite;

namespace GearSync.Database
{
    public class DatabaseInitializer
    {
        public static void Initialize(string databasePath)
        {
            if (!File.Exists(databasePath))
            {
                using var connection = new SqliteConnection($"Data Source={databasePath}");
                connection.Open();

                string schema = File.ReadAllText("Database/schema.sql");
                using var command = connection.CreateCommand();
                command.CommandText = schema;
                command.ExecuteNonQuery();
            }
        }
    }
}