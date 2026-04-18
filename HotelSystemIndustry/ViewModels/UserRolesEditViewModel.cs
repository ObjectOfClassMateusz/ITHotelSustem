namespace HotelSystemIndustry.ViewModels
{

    public class UserRolesEditRole
    {
        public string Name { get; set; } = string.Empty;

        public bool HasRole { get; set; }
    }
    
    public class UserRolesEditViewModel
    {
        public string Id { get; set; } = string.Empty;

        public IList<UserRolesEditRole> Roles { get; set; } = new List<UserRolesEditRole>();
    }

}