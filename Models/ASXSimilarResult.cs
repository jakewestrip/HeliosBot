using System;
using System.Collections.Generic;
using System.Text;

namespace HeliosBot.Models
{
    public class ASXSimilarResult : ASXResult
    {
        public List<Company> Companies { get; set; }
    }

    public class Company
    {
        public string Code { get; set; }
        public string Name_Full { get; set; }
        public string Principal_Activities { get; set; }
        public string Industry_Group_Name { get; set; }
        public string Sector_Name { get; set; }
        public Share Primary_Share { get; set; }
    }

    public class Share
    {
        public string Code { get; set; }
        public string Last_Price { get; set; }
        public string Open_Price { get; set; }
        public string Change_In_Percent { get; set; }
        public string Volume { get; set; }
        public string Average_Daily_Volume { get; set; }
        public string Number_Of_Shares { get; set; }
        public string Market_Cap { get; set; }
    }
}
