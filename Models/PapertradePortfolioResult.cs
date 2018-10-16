using System;
using System.Collections.Generic;
using System.Text;

namespace HeliosBot.Models
{
    public class PapertradePortfolioResult
    {
        public long UserId { get; set; }
        public double Money { get; set; }
        public Dictionary<string, long> Stocks { get; set; }
    }
}
