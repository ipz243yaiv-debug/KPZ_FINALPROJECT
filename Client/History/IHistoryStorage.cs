using System.Collections.Generic;

namespace Client.History
{
    public interface IHistoryStorage
    {
        void SaveMessage(string message);
        List<string> LoadHistory();
        void ClearHistory();
    }
}