namespace Team_12.Models
{
    public class Facility
    {
        public int FacilityId { get; set; } // Primary Key
        public string Name { get; set; }    // Facility name (e.g., Museum, Park)
        public string Description { get; set; } // Brief description of the facility
        public string Type { get; set; }    // Type (e.g., Museum, Park, Community Hall)
        public decimal PricePerHour { get; set; } // Pricing for booking (per hour)
        public int Capacity { get; set; }   // Maximum people allowed
        public bool IsAvailable { get; set; }  // Availability status
        public string Address { get; set; }    // Facility address
        public string ImageURL { get; set; } // Optional: for virtual tours
        public double Latitude { get; set; }  // Geographic location for proximity searches
        public double Longitude { get; set; } // Geographic location for proximity searches

        public ICollection<Booking> Bookings { get; set; }  // One-to-many relationship to bookings
        public ICollection<Rating> Ratings { get; set; }    // One-to-many relationship to ratings
    }
}
