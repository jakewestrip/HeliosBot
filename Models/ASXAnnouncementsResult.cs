using System;
using System.Collections.Generic;
using System.Text;

namespace HeliosBot.Models
{
    public class ASXAnnouncementsResult : ASXResult
    {
        public string Paging_next_url { get; set; }
        public List<Announcement> Data { get; set; }
    }

    public class Announcement
    {
        public string Id { get; set; }
        public string Document_Date { get; set; }
        public string Document_Release_Date { get; set; }
        public string Url { get; set; }
        public string Header { get; set; }
        public string Market_Sensitive { get; set; }
        public string Number_Of_Pages { get; set; }
        public string Size { get; set; }
    }
}
