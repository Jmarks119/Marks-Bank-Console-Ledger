using MarksBankLedger.Models;
using System;

namespace MarksBankLedger.DAL
{
    public interface IAccountRepository : IDisposable
    {
        Account GetAccountByGuid(Guid guid);
        void InsertAccount(Account account);
        void UpdateAccount(Account account);
        void DeleteAccount(Guid guid);
        void Save();
    }
}
