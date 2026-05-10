using System;
using System.Collections.Generic;
using System.IO;

namespace Client.History
{
    public class TextFileStorage : IHistoryStorage
    {
        private readonly string _filePath;

        public TextFileStorage(string fileName = "chat_history.txt")
        {
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        }

        public void SaveMessage(string message)
        {
            try
            {
                File.AppendAllText(_filePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}{Environment.NewLine}");
            }
            catch { }
        }

        public List<string> LoadHistory()
        {
            if (!File.Exists(_filePath))
                return new List<string>();

            try
            {
                return new List<string>(File.ReadAllLines(_filePath));
            }
            catch
            {
                return new List<string>();
            }
        }

        public void ClearHistory()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    File.Delete(_filePath);
                }
                catch { }
            }
        }
    }
}