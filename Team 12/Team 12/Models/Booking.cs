namespace Team_12.Models
{
    public class Booking
    {
        public int BookingId { get; set; }  // Primary Key
        public int FacilityId { get; set; } // Foreign Key to Facility
        public Facility Facility { get; set; } // Navigation Property
        public DateTime BookingDate { get; set; } // Date of booking
        public TimeSpan StartTime { get; set; }  // Start time of the event
        public TimeSpan EndTime { get; set; }    // End time of the event
        public string UserId { get; set; }      // Foreign Key to Customer
        public ApplicationUser User { get; set; }   // Navigation Property
        public decimal TotalCost { get; set; }   // Calculated total based on price and duration
    }

}
