using MarksBankLedger.Models;
using System;
using System.Net.Mail;

namespace MarksBankLedger.DAL
{
    public interface IAccountRepository : IDisposable
    {
        Account GetAccountByGuid(Guid guid);
        Account GetAccountByEmail(string email);
        void InsertAccount(Account account);
        void UpdateAccount(Account account);
        void DeleteAccount(Guid guid);
        void Save();
    }
}
