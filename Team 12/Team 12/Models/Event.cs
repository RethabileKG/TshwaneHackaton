using System;
using System.Collections.Generic;

namespace Team_12.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int FacilityId { get; set; }
        public Facility Facility { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal EventPrice { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}