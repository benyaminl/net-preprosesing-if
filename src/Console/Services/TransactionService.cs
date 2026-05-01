using System;
using System.Collections.Generic;
using PosApp.Domain.Entities;
using PosApp.Domain.Repositories;

namespace PosApp.Console.Services
{
    public class TransactionService
    {
        private readonly ITransactionRepository _repo;
        public TransactionService(ITransactionRepository repo) { _repo = repo; }

#if CLIENT_A
        public Transaction AddTransaction(Transaction t)
        {
            t.Status = TransactionStatus.Pending;
            t.CreatedAt = DateTime.UtcNow;
            t.TotalAmount = t.Quantity * t.UnitPrice;
            _repo.Add(t);
            _repo.UpdateStatus(new[] { t.Id }, TransactionStatus.Completed);
            t.Status = TransactionStatus.Completed;
            return t;
        }
#endif

#if CLIENT_B
        public Transaction AddPendingTransaction(Transaction t)
        {
            t.Status = TransactionStatus.Pending;
            t.CreatedAt = DateTime.UtcNow;
            t.TotalAmount = t.Quantity * t.UnitPrice;
            _repo.Add(t);
            return t;
        }

        public void FinalizeTransactions(IEnumerable<int> ids)
        {
            _repo.UpdateStatus(ids, TransactionStatus.Completed);
        }

        public IEnumerable<Transaction> GetPending() { return _repo.GetPending(); }
#endif
    }
}
