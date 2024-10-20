namespace Team_12.Models
{
    public class Attendee
    {
        public int AttendeeId { get; set; }
        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; }
        public string Name { get; set; }
        public string ClientType { get; set; }  // e.g., "Student", "Pensioner", etc.
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
