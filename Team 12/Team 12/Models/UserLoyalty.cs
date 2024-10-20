using Team_12.Models;

public class UserLoyalty
{
    public int UserLoyaltyId { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public int Points { get; set; } 
    public DateTime LastUpdated { get; set; } 
}
