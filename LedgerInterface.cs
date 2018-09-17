using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using Alba.CsConsoleFormat.ColorfulConsole;
using Alba.CsConsoleFormat.Fluent;
using static System.ConsoleColor;

namespace MarksBankLedger
{
    public static class LedgerInterface
    {
        public static void DisplayWelcome()
        {
            Colors.WriteLine(new Document {
                Width = Console.BufferWidth,
                TextAlign = TextAlign.Center,
                Padding = 2,
                Children = {
                    new FigletDiv() {Text = "Marks Bank", Align = Align.Center, Color = Green },
                    new Span("Welcome to the Marks Bank's console ledger access point. Please log in with your account in order to access " +
                             "your balance and withdraw or deposit funds. If you do not yet have an account with Marks Bank then creating one is as easy " +
                             "as providing an e-mail. Press any key to continue to the login menu.")
                    { Color = Blue }
                }
            });
            Console.CursorVisible = false;
            Console.ReadKey();
        }

        public static string DisplayMenu(Dictionary<string, Tuple<string, Action>> options, string user)
        {
            bool invalidSelection = false;

            while (true)
            {
                var menu = new Document
                {
                    Width = Console.BufferWidth,
                    TextAlign = TextAlign.Center,
                    Padding = 2,
                    Children = {
                        new Span($"Hello {user}. Please select an option below and press the listed key to execute it.\n") {Color = Blue },
                        new Grid {
                            Color = Green,
                            Align = Align.Center,
                            Columns = {
                                new Column {Width = GridLength.Char(1)},
                                new Column {Width = GridLength.Auto, MinWidth = 80}
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
                                        Children = { opt.Value.Item1 }
                                    }
                                })
                            }
                        },
                        invalidSelection ? new Span("Sorry, that is an invalid selection, please press another key") {Color = Red}
                                         : null,
                    }
                };

                Console.Clear();
                ConsoleRenderer.RenderDocument(menu);
                Console.CursorVisible = false;
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
    }
}
