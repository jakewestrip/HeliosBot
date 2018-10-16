using System;
using System.Collections.Generic;
using System.Text;

namespace HeliosBot.Models
{
    public class ASXAnnualReportResult : ASXResult
    {
        public string Code { get; set; }
        public string Name_Full { get; set; }
        public string Logo_Image_Url { get; set; }
        public List<Report> Latest_Annual_Reports { get; set; }
    }

    public class Report
    {
        public string Id { get; set; }
        public string Document_Date { get; set; }
        public string Document_Release_Date { get; set; }
        public string Url { get; set; }
        public string Header { get; set; }
        public string Number_Of_Pages { get; set; }
        public string Size { get; set; }
    }
}
