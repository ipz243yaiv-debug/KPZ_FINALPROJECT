using Server.DAL;
using System;
using System.Data.SQLite;

namespace ChatLibrary.DAL
{
    public class MessageRepository
    {
        private readonly DbConnectionManager _dbManager;

        public MessageRepository(DbConnectionManager dbManager)
        {
            _dbManager = dbManager;
        }

        public void SaveMessage(int senderId, int? targetId, string text)
        {
            using (var connection = _dbManager.GetConnection())
            {
                connection.Open();
                string insertQuery = "INSERT INTO Messages (SenderId, TargetId, Text) VALUES (@senderId, @targetId, @text);";
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@senderId", senderId);
                    command.Parameters.AddWithValue("@targetId", targetId.HasValue ? (object)targetId.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@text", text);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}