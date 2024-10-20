using System.ComponentModel.DataAnnotations;
namespace Team_12.DTOs
{
    public class BookingRequestDto
    {
        [Required]
        public int FacilityId { get; set; }
        public int? EventId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public DateTime BookingDate { get; set; }
        [Required]
        public TimeSpan StartTime { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "At least one attendee is required")]
        public List<AttendeeDto> Attendees { get; set; }
    }
}