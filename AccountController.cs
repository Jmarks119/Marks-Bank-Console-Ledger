using MarksBankLedger.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarksBankLedger
{
    static class AccountController
    {
        private static readonly IAccountRepository accountRepository;
        private static readonly ITransactionRepository transactionRepository;

        static AccountController()
        {
            BankContext context = new BankContext();
            accountRepository = new AccountRepository(context);
            transactionRepository = new TransactionRepository(context);
        }

        public static void HandleLogin()
        {
            while (true)
            {
                Dictionary<string, string> optionsDict = new Dictionary<string, string>()
                {
                    {"L", "Login to, or create, a bank account" },
                    {"Q", "Exit this program" }
                };
                string selection = LedgerInterface.DisplayMenu(optionsDict, "guest");
                if (selection == "Q")
                {
                    return;
                }
            }
        }
    }
}
