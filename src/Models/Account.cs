using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MarksBankLedger.Models
{
    [Table("Account")]
    public class Account
    {
        public Guid AccountId { get; set; }

        [Index(IsUnique = true)]
        public string AccountEmail { get; set; }

        public virtual List<Transaction> Transactions { get; set; }
    }
}
