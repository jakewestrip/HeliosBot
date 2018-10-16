using System;
using System.Collections.Generic;
using System.Text;

namespace HeliosBot.Models
{
    public class ASXCompanyResult : ASXResult
    {
        public string Code { get; set; }
        public string Name_Full { get; set; }
        public string Name_Short { get; set; }
        public string Name_Abbrev { get; set; }
        public string Principal_Activities { get; set; }
        public string Industry_Group_Name { get; set; }
        public string Sector_Name { get; set; }
        public string Listing_Date { get; set; }
        public string Web_Address { get; set; }
        public string Mailing_Address { get; set; }
        public string Phone_Number { get; set; }
        public string Fax_Number { get; set; }
        public string Registry_Name { get; set; }
        public string Registry_Address { get; set; }
        public string Registry_Phone_Number { get; set; }
        public string Foreign_Exempt { get; set; }
        public string Investor_Relations_Url { get; set; }
        public string Logo_Image_Url { get; set; }
        public string Primary_Share_Code { get; set; }
        public string Recent_Announcement { get; set; }
    }
}
