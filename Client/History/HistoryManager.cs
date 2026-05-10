using System.Collections.Generic;

namespace Client.History
{
    public class HistoryManager
    {
        private IHistoryStorage _storageStrategy;

        public HistoryManager(IHistoryStorage strategy)
        {
            _storageStrategy = strategy;
        }

        public void SetStrategy(IHistoryStorage newStrategy)
        {
            _storageStrategy = newStrategy;
        }

        public void Save(string message)
        {
            _storageStrategy?.SaveMessage(message);
        }

        public List<string> GetHistory()
        {
            if (_storageStrategy != null)
                return _storageStrategy.LoadHistory();

            return new List<string>();
        }
    }
}