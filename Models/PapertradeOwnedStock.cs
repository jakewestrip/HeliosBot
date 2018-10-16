using System;
using System.Collections.Generic;
using System.Text;

namespace HeliosBot.Models
{
    public class PapertradeOwnedStock
    {
        public int OwnedStockId { get; set; }
        public virtual PapertradeUser User { get; set; }
        public long UserId { get; set; }
        public string StockCode { get; set; }
        public long Shares { get; set; }
    }
}
