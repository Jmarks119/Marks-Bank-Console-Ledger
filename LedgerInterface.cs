using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using Alba.CsConsoleFormat.Fluent;
using static System.ConsoleColor;

namespace MarksBankLedger
{
    public static class LedgerInterface
    {
        public static void PrintWelcome()
        {
            Colorful.Console.WriteAscii("Marks Bank", System.Drawing.Color.DarkGreen);
            Colors.WriteLine("Welcome to the Marks Bank's console ledger access point. Please log in with your account in order to access ".Blue(),
                "your balance and withdraw or deposit funds. If you do not yet have an account with Marks Bank then creating one is as easy ".Blue(),
                "as providing an e-mail. Press any key to continue to the login menu.".Blue());
            Console.ReadKey();
        }

        public static string DisplayMenu(Dictionary<string, string> options, string user)
        {
            bool invalidSelection = false;

            while (true)
            {
                var menu = new Document
                {
                    Children = {
                        new Span($"Hello {user}. Please select an option below and press the listed key to execute it.") {Color = Blue },
                        new Grid {
                            Color = Green,
                            Columns = {
                                new Column {Width = GridLength.Char(1)},
                                new Column {Width = GridLength.Auto}
                            },
                            Children = {
                                options.Select(opt => new [] {
                                    new Cell
                                    {
                                        Children = { opt.Key }
                                    },
                                    new Cell
                                    {
                                        Align = Align.Right,
                                        Children = { opt.Value }
                                    }
                                })
                            }
                        },
                        invalidSelection ? new Span("Sorry, that is an invalid selection, please press another key") {Color = Red}
                                         : null
                    }
                };

                Console.Clear();
                ConsoleRenderer.RenderDocument(menu);
                string selection = Console.ReadKey().KeyChar.ToString().ToUpper();
                if (options.Keys.Contains(selection))
                {
                    return selection;
                }
                else
                {
                    invalidSelection = true;
                }
            }
        }
    }
}
