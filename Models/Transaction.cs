using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarksBankLedger.Models
{
    [Table("Transaction")]
    public class Transaction
    {
        public int TransactionId { get; set; }
        public decimal TransactionAmount { get; set; }
        [Column(Order = 0)]
        public DateTime TransactionTime { get; set; }

        public Guid AccountId { get; set; }
        public virtual Account Account { get; set; }
    }
}
