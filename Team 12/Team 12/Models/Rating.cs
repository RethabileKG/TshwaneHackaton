namespace Team_12.Models
{
    public class Rating
    {
        public int RatingId { get; set; }    // Primary Key
        public int FacilityId { get; set; }  // Foreign Key to Facility
        public Facility Facility { get; set; }  // Navigation Property
        public string UserId { get; set; }  // Foreign Key to Customer
        public ApplicationUser User { get; set; }   // Navigation Property
        public int Stars { get; set; }       // Rating from 1 to 5 stars
        public string Comments { get; set; } // Customer feedback
        public DateTime Date { get; set; }   // Date of the rating
    }

}
