using MarksBankLedger.DAL;
using MarksBankLedger.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MarksBankLedger
{
    static class AccountController
    {
        private static readonly IAccountRepository accountRepository;
        private static readonly ITransactionRepository transactionRepository;

        private static Account currentUser;

        private static bool exiting = false;

        static AccountController()
        {
            var connection = new SQLiteConnection("Data Source=:memory:;Version=3;New=True;");
            connection.Open();
            BankContext context = new BankContext(connection);
            context.Database.CreateIfNotExists();
            accountRepository = new AccountRepository(context);
            transactionRepository = new TransactionRepository(context);
        }

        public static void GuestMenu()
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
                    bool newAccount = false;
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
                    if (user == null)
                    {
                        if (LedgerInterface.DisplayConfirmation("An account with this email does not exist yet. Would you like to open an account?\n" + 
                            "Enter Y/y to confirm, or any other key to cancel."))
                        {
                            newAccount = true;
                        }
                        else
                        {
                            return;
                        }
                    }
                    string loginToken = GenerateLoginToken(6);
                    if (EmailService.SendLoginEmail(mailAddress, loginToken))
                    {
                        for (int r = 0; r < 3; r++)
                        {
                            string response = LedgerInterface.DisplayPrompt("Please enter the login token in your provided email, 3 failures will cancel the login\n" +
                                                                            "(This can take some time, Yahoo's SMTP server does not seem to be very fast)\n" +
                                                                            $"DEV NOTE: This actually takes much too long to be usable. For convenince's sake your login token is {loginToken}\n" +
                                                                            "But you can note it down and check your inbox later to see that the passwordless authentication does work.\n");
                            if (response == loginToken)
                            {
                                if (newAccount)
                                {
                                    user = CreateNewBankAccount(email);
                                }
                                currentUser = user;
                                return;
                            }
                        }
                        return;
                    }
                    else
                    {
                        LedgerInterface.DisplayConfirmation("Something went wrong in sending your login email. Please contact a Marks Bank admin.");
                    }
                }
                catch (ArgumentException)
                {
                    if (!LedgerInterface.DisplayConfirmation("You must enter an email address to login. Press Y/y to try again, or any other key to cancel."))
                    {
                        return;
                    }
                }
                catch (FormatException)
                {
                    if (!LedgerInterface.DisplayConfirmation("That email address was not in an accepted format. Press Y/y to try again, or any other key to cancel."))
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

        private static string GenerateLoginToken(int length)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
            StringBuilder token = new StringBuilder();
            using (RNGCryptoServiceProvider numgen = new RNGCryptoServiceProvider())
            {
                while (token.Length < length)
                {
                    byte[] rByte = new byte[1];
                    numgen.GetBytes(rByte);
                    char rChar = (char)rByte[0];
                    if (validChars.Contains(rChar))
                    {
                        token.Append(rChar);
                    }
                }
            }
            return token.ToString();
        }

        private static Account CreateNewBankAccount(string accountEmail)
        {
            Account account = new Account
            {
                AccountId = Guid.NewGuid(),
                AccountEmail = accountEmail
            };
            accountRepository.InsertAccount(account);
            accountRepository.Save();
            return account;
        }
    }
}
