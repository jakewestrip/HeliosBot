using System;
using System.Collections.Generic;
using System.Text;

namespace HeliosBot.Models
{
    public class PapertradeUser
    {
        public long UserId { get; set; }
        public double Money { get; set; }
        public virtual ICollection<PapertradeOwnedStock> OwnedStocks { get; set; }
        public virtual ICollection<PapertradeTransaction> Transactions { get; set; }
    }
}
