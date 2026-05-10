using System.Data.SQLite;
using System.IO;

namespace Server.DAL
{
    public class DbConnectionManager
    {
        private const int DefaultTimeout = 30;
        private const string DefaultDbName = "ChatDatabase.sqlite";

        private readonly DatabaseConfig _config;
        private readonly string _connectionString;

        public DbConnectionManager(DatabaseConfig config)
        {
            _config = config;
            _connectionString = $"Data Source={_config.DatabaseName};Version=3;Default Timeout={_config.TimeoutSeconds};Foreign Keys={(_config.UseForeignKeys ? "True" : "False")};";
        }

        public void InitializeDatabase()
        {
            if (!File.Exists(_config.DatabaseName))
            {
                SQLiteConnection.CreateFile(_config.DatabaseName);
            }

            using (var connection = GetConnection())
            {
                connection.Open();

                string createUsersTableQuery = @"CREATE TABLE IF NOT EXISTS Users (
                                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                    Username TEXT UNIQUE NOT NULL,
                                                    PasswordHash TEXT NOT NULL
                                                );";

                string createMessagesTableQuery = @"CREATE TABLE IF NOT EXISTS Messages (
                                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                        SenderId INTEGER NOT NULL,
                                                        TargetId INTEGER,
                                                        Text TEXT NOT NULL,
                                                        Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                                                        FOREIGN KEY(SenderId) REFERENCES Users(Id)
                                                    );";

                using (var command = new SQLiteCommand(createUsersTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
                using (var command = new SQLiteCommand(createMessagesTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(_connectionString);
        }
    }
}