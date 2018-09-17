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

        private static bool exiting = false;

        static AccountController()
        {
            BankContext context = new BankContext();
            accountRepository = new AccountRepository(context);
            transactionRepository = new TransactionRepository(context);
        }

        public static void TopMenu()
        {
            while (true)
            {
                Dictionary<string, Tuple<string, Action>> optionsDict = new Dictionary<string, Tuple<string, Action>>()
                {
                    {"L", new Tuple<string, Action>("Login to, or create, a bank account", HandleLogin) },
                    {"Q", new Tuple<string, Action>("Exit this program", Exit) }
                };
                string selection = LedgerInterface.DisplayMenu(optionsDict, "guest");
                optionsDict[selection].Item2.Invoke(); //Call the action in the tuple for the selected Dictionary entry.
                if (exiting)
                {
                    return;
                }
            }
        }

        private static void HandleLogin()
        {

        }

        private static void Exit()
        {
            exiting = true;
        }
    }
}
