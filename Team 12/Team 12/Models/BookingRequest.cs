using System;
using System.Collections.Generic;

namespace Team_12.Models
{
    public class BookingRequest
    {
        public int FacilityId { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public DateTime BookingDate { get; set; }
        public int? EventId { get; set; }  // New property
        public Event Event { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public decimal TotalCost { get; set; }
        public List<string> ClientTypes { get; set; }
    }
}
