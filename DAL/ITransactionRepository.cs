using MarksBankLedger.Models;
using System;
using System.Collections.Generic;

namespace MarksBankLedger.DAL
{
    public interface ITransactionRepository : IDisposable
    {
        IEnumerable<Transaction> GetRecentTransactionsByAccountId(Guid guid, int logsize);
        decimal GetAccountBalance(Guid guid);
        void InsertTransaction(Transaction transaction);
        void UpdateTransaction(Transaction transaction);
        void DeleteTransaction(int transactionId);
        void Save();
    }
}
