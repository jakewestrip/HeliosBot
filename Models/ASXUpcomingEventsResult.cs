using System;
using System.Collections.Generic;
using System.Text;

namespace HeliosBot.Models
{
    public class ASXUpcomingEventsResult : ASXResult
    {
        public List<Event> Data { get; set; }
    }

    public class Event
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Start_Date { get; set; }
        public string Url { get; set; }
    }
}
