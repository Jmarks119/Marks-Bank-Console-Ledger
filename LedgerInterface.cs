using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Drawing.Color;
using Alba.CsConsoleFormat;
using Alba.CsConsoleFormat.Fluent;

namespace MarksBankLedger
{
    public class LedgerInterface
    {
        public static void PrintWelcome()
        {
            Colorful.Console.WriteAscii("Marks Bank", DarkGreen);
            Colors.WriteLine("Welcome to the Marks Bank's console ledger access point. Please log in with your account in order to access ".Blue(),
                "your balance and withdraw or deposit funds. If you do not yet have an account with Marks Bank then creating one is as easy ".Blue(),
                "as providing an e-mail. Press any key to continue to the main menu.".Blue());
            Console.ReadKey();
        }
    }
}
