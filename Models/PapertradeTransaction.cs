using System;
using System.Collections.Generic;
using System.Text;

namespace HeliosBot.Models
{
    public enum PapertradeTransactionType
    {
        BUY,
        SELL
    }

    public class PapertradeTransaction
    {
        public int TransactionId { get; set; }
        public virtual PapertradeUser User { get; set; }
        public long UserId { get; set; }
        public string StockCode { get; set; }
        public long Shares { get; set; }
        public decimal Price { get; set; }
        public PapertradeTransactionType TransactionType { get; set; }
    }
}
