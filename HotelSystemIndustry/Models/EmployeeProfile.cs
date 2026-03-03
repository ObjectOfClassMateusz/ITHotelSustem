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
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public EmployeeRole Role { get; set; }
    }
}
