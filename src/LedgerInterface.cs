﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using Alba.CsConsoleFormat.ColorfulConsole;
using Alba.CsConsoleFormat.Fluent;
using MarksBankLedger.Models;
using static System.ConsoleColor;

namespace MarksBankLedger
{
    public static class LedgerInterface
    {
        internal static void DisplayWelcome()
        {
            ConsoleRenderer.RenderDocument(new Document {
                Width = Console.BufferWidth,
                TextAlign = TextAlign.Center,
                Padding = 2,
                Children =
                {
                    new FigletDiv() {Text = "Marks Bank", Align = Align.Center, Color = Green },
                    new Span("Welcome to the Marks Bank's console ledger access point. Please log in with your account in order to access " +
                             "your balance and withdraw or deposit funds. If you do not yet have an account with Marks Bank then creating one is as easy " +
                             "as providing an e-mail. Press any key to continue to the login menu.") { Color = Blue }
                }
            });
            Console.ReadKey();
        }

        internal static void DisplayGoodbye()
        {
            ConsoleRenderer.RenderDocument(new Document
            {
                Width = Console.BufferWidth,
                TextAlign = TextAlign.Center,
                Padding = 2,
                Children =
                {
                    new FigletDiv() {Text = "Marks Bank", Align = Align.Center, Color = Green },
                    new Span("Thank you for your continued usage of Marks Bank. Have a great day.") { Color = Blue }
                }
            });
            Console.ReadKey();
        }

        internal static string DisplayMenu(Dictionary<string, Tuple<string, Action>> options, string user)
        {
            bool invalidSelection = false;

            while (true)
            {
                Console.Clear();
                ConsoleRenderer.RenderDocument(new Document
                {
                    Width = Console.BufferWidth,
                    TextAlign = TextAlign.Center,
                    Padding = 2,
                    Children =
                    {
                        new Span($"Hello {user}. Please select an option below and press the listed key to execute it.\n") {Color = Blue },
                        new Grid
                        {
                            Color = Green,
                            Align = Align.Center,
                            Columns =
                            {
                                new Column {Width = GridLength.Char(1)},
                                new Column {Width = GridLength.Auto}
                            },
                            Children =
                            {
                                options.Select(opt => new []
                                {
                                    new Cell(opt.Key),
                                    new Cell(opt.Value.Item1) {Align = Align.Right}
                                })
                            }
                        },
                        invalidSelection ? new Span("Sorry, that is an invalid selection, please press another key") {Color = Red}
                                         : null,
                    }
                });
                string selection = Console.ReadKey().KeyChar.ToString().ToUpper();
                if (options.Keys.Contains(selection))
                {
                    return selection;
                }
                else
                {
                    invalidSelection = true; // Flip the invalid selection boolean so that the menu re-renders with an error message.
                }
            }
        }

        internal static string DisplayPrompt(string prompt)
        {
            bool returning = false;
            StringBuilder entry = new StringBuilder();
            while (!returning)
            {
                Console.Clear();
                ConsoleRenderer.RenderDocument(
                new Document()
                {
                    TextAlign = TextAlign.Center,
                    Padding = 2,
                    Children = {
                        new Span(prompt) {Color = DarkYellow},
                        new Span(entry.ToString()) {Color = Green} 
                    }
                });
                char character = Console.ReadKey().KeyChar;
                if (character == '\r')
                {
                    returning = true;
                }
                else if (character == '\b')
                {
                    entry.Remove(entry.Length - 1, 1);
                }
                else
                {
                    entry.Append(character);
                }
            }
            return entry.ToString();
        }

        /// <summary>
        /// A specific prompt for getting a yes/no confirmation from the user.
        /// </summary>
        /// <param name="prompt">A message to display to the user for them to reply yes or no to.</param>
        /// <returns>true if the user selects y, false if any other key is pressed.</returns>
        internal static bool DisplayConfirmation(string prompt)
        {
            Console.Clear();
            ConsoleRenderer.RenderDocument(new Document()
            {
                TextAlign = TextAlign.Center,
                Color = Red,
                Padding = 2,
                Children = { new Span(prompt) }
            });
            char entry = Console.ReadKey().KeyChar;
            if (entry == 'y' || entry == 'Y')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static void DisplayTransactionHistory(string user, decimal balance, IEnumerable<Transaction> transactions)
        {
            Console.Clear();
            ConsoleRenderer.RenderDocument(new Document()
            {
                Padding = 2,
                TextAlign = TextAlign.Center,
                Color = DarkYellow,
                Children =
                {
                    new Span($"Here is a list of your recent transactions "), new Span(user) {Color = Blue },
                    new Span("\nYour current balance is "), new Span(string.Format("{0:C2}", balance)) {Color = Green },
                    new Grid()
                    {
                        Align = Align.Center,
                        Color = Blue,
                        Columns =
                        {
                            new Column {Width = GridLength.Auto },
                            new Column {Width = GridLength.Auto },
                        },
                        Children =
                        {
                            new Cell("Transaction Time"),
                            new Cell("Transaction Amount"),
                            transactions.Select(xact => new[]
                            {
                                new Cell(xact.TransactionTime) {Background = (xact.TransactionAmount > 0 ? DarkGreen : DarkRed), Color = Gray },
                                new Cell(String.Format("{0:C2}", xact.TransactionAmount)) {TextAlign = TextAlign.Right, Background = (xact.TransactionAmount > 0 ? DarkGreen : DarkRed), Color = Gray }
                            })
                        }
                    },
                    new Span("\nPress any key to return to your account menu.")
                }
            });
            Console.ReadKey();
        }

        internal static decimal DisplayTransactionPrompt(decimal currentBalance, string transactionType)
        {
            bool returning = false;
            bool invalidEntry = false;
            StringBuilder entry = new StringBuilder();
            decimal entryDec = 0;
            while (!returning)
            {
                const string currencyChars = "0123456789.";
                Console.Clear();
                ConsoleRenderer.RenderDocument(new Document()
                    {
                        TextAlign = TextAlign.Center,
                        Color = DarkYellow,
                        Padding = 2,
                        Children =
                    {
                        new Span("Your current account balance is "), new Span(String.Format("{0:C2}", currentBalance) + "\n") {Color = Green},
                        new Span($"Please enter the amount of your {transactionType} (enter 0 to cancel): "),
                        new Span("$" + entry.ToString() + "\n") {Color = (transactionType == "deposit" ? Green : Red)},
                        invalidEntry ? new Span("Invalid Entry: You must enter a number of 28 or less digits with up to two decimal places.")
                                     : null
                    }
                });
                char character = Console.ReadKey().KeyChar;
                if (character == '\r')
                {
                    if (decimal.TryParse(entry.ToString(), out entryDec))
                    {
                        if (Decimal.Round(entryDec, 2) == entryDec) // This logic checks that the user hasn't entered a number with more than 2 decimal places.
                        {
                            returning = true;
                        }
                        else
                        {
                            invalidEntry = true;
                        }
                    }
                    else
                    {
                        invalidEntry = true;
                    }
                }
                else if (character == '\b')
                {
                    entry.Remove(entry.Length - 1, 1);
                }
                else if (currencyChars.Contains(character))
                {
                    entry.Append(character);
                }
            }
            return entryDec;
        }
    }
}
