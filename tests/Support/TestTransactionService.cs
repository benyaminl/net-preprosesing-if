using System;
using System.Collections.Generic;
using PosApp.Domain.Entities;
using PosApp.Domain.Repositories;

namespace PosApp.Tests.Support
{
    public class TestTransactionService
    {
        private readonly ITransactionRepository _repo;
        public TestTransactionService(ITransactionRepository repo) { _repo = repo; }

        public Transaction AddTransactionClientA(Transaction t)
        {
            t.Status = TransactionStatus.Pending;
            t.CreatedAt = DateTime.UtcNow;
            t.TotalAmount = t.Quantity * t.UnitPrice;
            _repo.Add(t);
            _repo.UpdateStatus(new[] { t.Id }, TransactionStatus.Completed);
            t.Status = TransactionStatus.Completed;
            return t;
        }

        public Transaction AddPendingTransactionClientB(Transaction t)
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
    }
}
