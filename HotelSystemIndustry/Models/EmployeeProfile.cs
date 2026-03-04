using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models
{
    public enum EmployeeRole
    {
        Reception,
        Manager,//Finance
        Moderator
    }
    public class EmployeeProfile
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public EmployeeRole Role { get; set; }
    }
}
