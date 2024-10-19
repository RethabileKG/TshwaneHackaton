namespace Team_12.Models
{
    public class QRVerificationModel
    {
        public int BookingId { get; set; }
        public int FacilityId { get; set; }
        public DateTime BookingDate { get; set; }
        public bool IsUsed { get; set; }

        // Navigation properties
        public Booking Booking { get; set; }
        public Facility Facility { get; set; }
    }
}