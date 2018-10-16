using System;
using System.Collections.Generic;
using System.Text;

namespace HeliosBot.Models
{
    public class PapertradeBuyResult
    {
        public long UserId { get; set; }
        public string StockCode { get; set; }
        public long Amount { get; set; }
        public double Price { get; set; }
        public double Cost { get; set; }
        public double Money { get; set; }
        public long TotalAmount { get; set; }
    }
}
