using Team_12.Models;

public class UserLoyalty
{
    public int UserLoyaltyId { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public int Points { get; set; } // Track points for each user
    public DateTime LastUpdated { get; set; } // To track when points were last added
}
