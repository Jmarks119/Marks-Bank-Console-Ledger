using MarksBankLedger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MarksBankLedger.DAL
{
    public class AccountRepository: IAccountRepository
    {
        private readonly BankContext _context;

        public AccountRepository(BankContext context)
        {
            _context = context;
        }

        public Account GetAccountByGuid(Guid guid)
        {
            return _context.Accounts.Find(guid);
        }

        public Account GetAccountByEmail(string email)
        {
            return _context.Accounts.Where(x => x.AccountEmail == email).SingleOrDefault();
        }

        public void InsertAccount(Account account)
        {
            _context.Accounts.Add(account);
        }

        public void UpdateAccount(Account account)
        {
            _context.Entry(account).State = System.Data.Entity.EntityState.Modified;
        }

        public void DeleteAccount(Guid guid)
        {
            Account account = _context.Accounts.Find(guid);
            _context.Accounts.Remove(account);
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
