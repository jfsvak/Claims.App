using System;

namespace Claims.Web.Models
{
    public class Event
    {
        public string Id { get; set; }
        public string Vendor { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}