namespace Team_12.Models
{
    public class RegisterModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Birthday { get; set; }
        public string Role { get; set; } // "admin" or "non-admin"
    }

}
