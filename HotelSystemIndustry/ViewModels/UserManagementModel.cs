namespace HotelSystemIndustry.ViewModels
{
    public class UserManagementInfo
    {
        public string Id { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool EmailVerified { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class UserManagementModel
    {
        public IList<UserManagementInfo> Users { get; set; } = new List<UserManagementInfo>();
    }
}