using System.Collections.Generic;
using System.Linq;
using PosApp.Domain.Entities;
using PosApp.Domain.Repositories;

namespace PosApp.Tests.Support
{
    public class InMemoryTransactionRepository : ITransactionRepository
    {
        private readonly List<Transaction> _store = new List<Transaction>();
        private int _nextId = 1;

        public int Add(Transaction t)
        {
            t.Id = _nextId++;
            _store.Add(t);
            return t.Id;
        }

        public IEnumerable<Transaction> GetPending()
        {
            return _store.Where(t => t.Status == TransactionStatus.Pending).ToList();
        }

        public void UpdateStatus(IEnumerable<int> ids, TransactionStatus status)
        {
            var idSet = new HashSet<int>(ids);
            foreach (var t in _store.Where(t => idSet.Contains(t.Id)))
                t.Status = status;
        }

        public void Seed(IEnumerable<Transaction> transactions)
        {
            foreach (var t in transactions)
            {
                t.Id = _nextId++;
                _store.Add(t);
            }
        }
    }
}
