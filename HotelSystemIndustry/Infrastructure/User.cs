using Microsoft.AspNetCore.Identity;


namespace HotelSystemIndustry.Infrastructure;

public class User : IdentityUser
{
    public string FullName { get; set; } = "";
}

