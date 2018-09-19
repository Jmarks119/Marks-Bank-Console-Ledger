using MarksBankLedger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarksBankLedger.DAL
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly BankContext _context;

        public TransactionRepository(BankContext context)
        {
            _context = context;
        }

        public IEnumerable<Transaction> GetRecentTransactionsByAccountId(Guid guid, int logsize)
        {
            return _context.Accounts.Find(guid).Transactions.OrderByDescending(x => x.TransactionTime).Take(logsize);
        }

        public decimal GetAccountBalance(Guid guid)
        {
            return _context.Accounts.Find(guid).Transactions.Select(x => x.TransactionAmount).DefaultIfEmpty().Sum();
        }

        public void InsertTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
        }

        public void UpdateTransaction(Transaction transaction)
        {
            _context.Entry(transaction).State = System.Data.Entity.EntityState.Modified;
        }

        public void DeleteTransaction(int transactionId)
        {
            Transaction transaction = _context.Transactions.Find(transactionId);
            _context.Transactions.Remove(transaction);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
