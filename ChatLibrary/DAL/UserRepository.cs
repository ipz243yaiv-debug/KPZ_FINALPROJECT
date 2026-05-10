using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Server.DAL
{
    public class UserRepository
    {
        private readonly DbConnectionManager _dbManager;
        private const int MaxUsernameLength = 50;

        public UserRepository(DbConnectionManager dbManager)
        {
            _dbManager = dbManager;
        }

        public bool RegisterUser(string username, string passwordHash)
        {
            if (string.IsNullOrEmpty(username) || username.Length > MaxUsernameLength || CheckUserExists(username))
            {
                return false;
            }

            using (var connection = _dbManager.GetConnection())
            {
                connection.Open();
                string query = "INSERT INTO Users (Username, PasswordHash) VALUES (@username, @passwordHash);";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@passwordHash", passwordHash);
                    command.ExecuteNonQuery();
                }
                return true;
            }
        }

        public int? Authenticate(string username, string passwordHash)
        {
            using (var connection = _dbManager.GetConnection())
            {
                connection.Open();
                string query = "SELECT Id FROM Users WHERE Username = @username AND PasswordHash = @passwordHash;";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@passwordHash", passwordHash);

                    var result = command.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : (int?)null;
                }
            }
        }

        public bool UpdateNickname(int id, string newName)
        {
            if (string.IsNullOrEmpty(newName) || newName.Length > MaxUsernameLength || CheckUserExists(newName))
            {
                return false;
            }

            using (var connection = _dbManager.GetConnection())
            {
                connection.Open();
                string query = "UPDATE Users SET Username = @name WHERE Id = @id";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", newName);
                    command.Parameters.AddWithValue("@id", id);
                    int rows = command.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }

        private bool CheckUserExists(string name)
        {
            using (var connection = _dbManager.GetConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @name";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    var result = command.ExecuteScalar();
                    return result != null && Convert.ToInt32(result) > 0;
                }
            }
        }
    }
}