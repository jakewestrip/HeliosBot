using System;

namespace HeliosBot.Models
{
    public class ASXLookupResult : ASXResult
    {
        public string Code { get; set; }
        public string Isin_Code { get; set; }
        public string Desc_Full { get; set; }
        public double? Last_Price { get; set; }
        public double? Open_Price { get; set; }
        public double? Day_High_Price { get; set; }
        public double? Day_Low_Price { get; set; }
        public double? Change_Price { get; set; }
        public string Change_In_Percent { get; set; }
        public long? Volume { get; set; }
        public double? Bid_Price { get; set; }
        public double? Offer_Price { get; set; }
        public double? Previous_Close_Price { get; set; }
        public string Previous_Day_Percentage_Change { get; set; }
        public double? Year_High_Price { get; set; }
        public DateTime? Last_Trade_Date { get; set; }
        public DateTime? Year_High_Date { get; set; }
        public double? Year_Low_Price { get; set; }
        public DateTime? Year_Low_Date { get; set; }
        public double? Year_Open_Price { get; set; }
        public DateTime? Year_Open_Date { get; set; }
        public double? Year_Change_Price { get; set; }
        public string Year_Change_In_Percentage { get; set; }
        public double? PE { get; set; }
        public double? EPS { get; set; }
        public long? Average_Daily_Volume { get; set; }
        public long? Annual_Dividend_Yield { get; set; }
        public long? Market_Cap { get; set; }
        public long? Number_Of_Shares { get; set; }
        public bool? Suspended { get; set; }
    }
}
