using Microsoft.AspNetCore.Identity;

namespace Team_12.Models
{
   
    public class ApplicationUser : IdentityUser
    {
        // Add additional properties here
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime DateRegistered { get; set; } = DateTime.UtcNow;

    }
}