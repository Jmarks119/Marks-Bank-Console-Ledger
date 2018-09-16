using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarksBankLedger.Models
{
    public class Account
    {
        public Guid AccountId { get; set; }
        public string AccountEmail { get; set; }

        public virtual List<Transaction> Transactions { get; set; }
    }
}
