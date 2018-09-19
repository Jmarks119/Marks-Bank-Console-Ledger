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
        private static bool toTop = false;

        static AccountController()
        {
            var connection = new SQLiteConnection("Data Source=:memory:;Version=3;New=True;");
            connection.Open();
            BankContext context = new BankContext(connection);
            context.Database.CreateIfNotExists();
            accountRepository = new AccountRepository(context);
            transactionRepository = new TransactionRepository(context);
        }

        internal static void MenuTop()
        {
            while (!exiting)
            {
                toTop = false;
                if (currentUser != null)
                {
                    MemberMenu();
                }
                else
                {
                    GuestMenu();
                }
            }
        }

        private static void GuestMenu()
        {
            while (!(exiting || toTop))
            {
                Dictionary<string, Tuple<string, Action>> optionsDict = new Dictionary<string, Tuple<string, Action>>()
                {
                    {"L", new Tuple<string, Action>("Login to, or create, a bank account", HandleLogin) },
                    {"Q", new Tuple<string, Action>("Exit this program", Exit) }
                };
                string selection = LedgerInterface.DisplayMenu(optionsDict, "guest");
                optionsDict[selection].Item2.Invoke(); //Call the action in the tuple for the selected Dictionary entry.
            }
            return;
        }

        private static void MemberMenu()
        {
            while (!(exiting || toTop))
            {
                Dictionary<string, Tuple<string, Action>> optionsDict = new Dictionary<string, Tuple<string, Action>>()
                {
                    {"V", new Tuple<String, Action>("View your account balance and transaction history", TransactionHistory) },
                    {"D", new Tuple<string, Action>("Make a deposit to your account", AddDeposit) },
                    {"W", new Tuple<string, Action>("Make a withdrawal from your account", AddWithdrawal) },
                    {"O", new Tuple<string, Action>("Log out of your account and return to the login menu", Logout) },
                    {"Q", new Tuple<string, Action>("Exit this program", Exit) }

                };
                string selection = LedgerInterface.DisplayMenu(optionsDict, currentUser.AccountEmail);
                optionsDict[selection].Item2.Invoke(); //Call the action in the tuple for the selected Dictionary entry.
            }
            return;
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
                                toTop = true;
                                return;
                            }
                        }
                        return;
                    }
                    else
                    {
                        LedgerInterface.DisplayConfirmation("Something went wrong in sending your login email. Please contact a Marks Bank admin.\n" +
                                                            "(Sometimes the email fails for reasons I haven't quite tracked down. Try again and it'll probably work)");
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

        private static void TransactionHistory()
        {
            throw new NotImplementedException();
        }

        private static void AddDeposit()
        {
            decimal amount = LedgerInterface.DisplayTransactionPrompt(transactionRepository.GetAccountBalance(currentUser.AccountId), "deposit");
            if (amount != 0)
            {
                Transaction deposit = new Transaction()
                {
                    TransactionAmount = amount,
                    Account = currentUser,
                    TransactionTime = DateTime.Now
                };
                transactionRepository.InsertTransaction(deposit);
                transactionRepository.Save();
            }
            return;
        }

        private static void AddWithdrawal()
        {
            decimal balance = transactionRepository.GetAccountBalance(currentUser.AccountId);
            decimal amount = LedgerInterface.DisplayTransactionPrompt(balance, "withdrawal");
            if (amount != 0)
            {
                if (amount < balance)
                {
                    Transaction deposit = new Transaction()
                    {
                        TransactionAmount = -amount,
                        Account = currentUser,
                        TransactionTime = DateTime.Now
                    };
                    transactionRepository.InsertTransaction(deposit);
                    transactionRepository.Save();
                }
                else
                {
                    LedgerInterface.DisplayConfirmation("Your account is currently restricted to maintaining a non-negative balance.\n" +
                                                        "You may contact a Marks Bank account manager about your withdrawal limit");
                }
            }
            return;
        }

        private static void Logout()
        {
            currentUser = null;
            toTop = true;
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
                AccountEmail = accountEmail,
                Transactions = new List<Transaction>()
            };
            accountRepository.InsertAccount(account);
            accountRepository.Save();
            return account;
        }
    }
}
