using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarksBankLedger
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            LedgerInterface.DisplayWelcome();
            AccountController.GuestMenu();
        }
    }
}
