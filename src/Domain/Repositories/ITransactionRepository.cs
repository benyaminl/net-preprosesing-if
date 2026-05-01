using System.Collections.Generic;
using PosApp.Domain.Entities;

namespace PosApp.Domain.Repositories
{
    public interface ITransactionRepository
    {
        int Add(Transaction transaction);
        IEnumerable<Transaction> GetPending();
        void UpdateStatus(IEnumerable<int> ids, TransactionStatus status);
    }
}
