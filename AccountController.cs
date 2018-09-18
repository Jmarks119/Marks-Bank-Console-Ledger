using MarksBankLedger.DAL;
using MarksBankLedger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
            while (true)
            {
                try
                {
                    string email = LedgerInterface.DisplayPrompt("Please enter your email address: ").ToLower();
                    string emailConfirm = LedgerInterface.DisplayPrompt("Please enter your email again to confirm: ").ToLower();
                    if (!email.Equals(emailConfirm))
                    {
                        if (!LedgerInterface.DisplayConfirmation("Your email entries do not match. Press Y/y to try again, or any other key to cancel."))
                        {
                            return;
                        }
                    }
                    MailAddress mailAddress = new MailAddress(email);
                    Account user = accountRepository.GetAccountByEmail(email);
                }
                //catch (ArgumentException)
                //{
                //    if (!LedgerInterface.DisplayConfirmation("You must enter an email address to login. Press Y/y to try again, or any other key to cancel."))
                //    {
                //        return;
                //    }
                //}
                catch (FormatException)
                {
                    if (!LedgerInterface.DisplayConfirmation("That email was not in an accepted format. Press Y/y to try again, or any other key to cancel."))
                    {
                        return;
                    }
                }
            }
        }

        private static void Exit()
        {
            exiting = true;
        }
    }
}
