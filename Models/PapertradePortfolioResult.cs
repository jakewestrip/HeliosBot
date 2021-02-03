using System;
using System.Collections.Generic;
using System.Text;

namespace HeliosBot.Models
{
    public class PapertradePortfolioResult
    {
        public long UserId { get; set; }
        public double Money { get; set; }
        public List<PapertradePortfolioHolding> Holdings { get; set; }
    }

    public class PapertradePortfolioHolding
    {
        public string StockCode { get; set; }
        public decimal LastPrice { get; set; }
        public long Quantity { get; set; }
        public decimal PricePaid { get; set; }
        public string TotalGain { get; set; }
        public string TotalGainPercent { get; set; }
        public string Value { get; set; }
    }
}
