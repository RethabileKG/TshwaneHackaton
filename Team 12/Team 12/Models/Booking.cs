using System;
using System.Collections.Generic;

namespace Team_12.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int FacilityId { get; set; }
        public Facility Facility { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public decimal TotalCost { get; set; }
        public decimal DiscountApplied { get; set; }
        public decimal FinalPrice { get; set; }
        public List<string> ClientTypes { get; set; }
        public string Status { get; set; }
        public bool IsFreeBooking { get; set; }
        public string QRCode { get; set; } // New property for storing the QR code

        public bool IsUsed { get; set; }
        public DateTime? UsedDateTime { get; set; }

        // Navigation property
        public virtual ICollection<Booking> Bookings { get; set; }

        // Navigation property for loyalty
        public virtual UserLoyalty UserLoyalty { get; set; }
    }
}
