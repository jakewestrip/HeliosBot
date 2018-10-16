using System;
using System.Collections.Generic;
using System.Text;

namespace HeliosBot.Models
{
    public class ASXHighchartResult : ASXResult
    {
        public DateTime Date { get; set; }
        public double Price { get; set; }
        public long Volume { get; set; }
    }

    class ASXHighchartDatapoint
    {

    }
}
