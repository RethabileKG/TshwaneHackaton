using System.ComponentModel.DataAnnotations;

public class AttendeeDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string ClientType { get; set; }
    [Phone]
    public string PhoneNumber { get; set; }
}