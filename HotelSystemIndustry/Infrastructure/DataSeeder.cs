using Microsoft.AspNetCore.Identity;

namespace HotelSystemIndustry.Infrastructure;


public class DataSeeder
{
    public static async Task SeedDatabase(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        HotelDbContext context = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DataSeeder>>();

        try
        {
            logger.LogInformation("Upewnianie się, że baza danych jest stworzona");
            await context.Database.EnsureCreatedAsync();

            logger.LogInformation("Seedowanie roli użytkowników");
            await AddRoleAsync(roleManager, "Admin");
            await AddRoleAsync(roleManager, "HotelEmployee");
            await AddRoleAsync(roleManager, "KitchenEmployee");
            await AddRoleAsync(roleManager, "MaintainanceEmployee");
            await AddRoleAsync(roleManager, "TradingEmployee");
            await AddRoleAsync(roleManager, "RecreationEmployee");

            await AddUserAsync(userManager, "admin@admin.com", "Właściciel Hotelu", "admin123$N", "Admin");
            await AddUserAsync(userManager, "hotellady@hotel.com", "Anna Machelska", "admin123$N", "HotelEmployee");
            await AddUserAsync(userManager, "kitchenlady@kitchen.com", "Anna Niedzielska", "admin123$N", "KitchenEmployee");
            await AddUserAsync(userManager, "maintainanceguy@maintainance.com", "Marek Niedzielski", "admin123$N", "MaintainanceEmployee");
            await AddUserAsync(userManager, "tradingperson@trading.com", "Orestes Niedzielski", "admin123$N", "TradingEmployee");
            await AddUserAsync(userManager, "recreationguy@recreation.com", "Sławomir Niedzielski", "admin123$N", "RecreationEmployee");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Wystąpił błąd przy seedowaniu bazy danych");
        }
    }

    private static async Task AddUserAsync(UserManager<User> userManager, string email, string fullname, string password, string role)
    {
        if (await userManager.FindByEmailAsync(email) == null)
        {
            var newUser = new User
            {
                FullName = fullname,
                UserName = email,
                NormalizedUserName = email.ToUpper(),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await userManager.CreateAsync(newUser, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newUser, role);
            }
            else
            {
                throw new Exception($"Błąd przy tworzeniu użytkownika o roli: {role}");
            }
        }
    }

    private static async Task AddRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                throw new Exception($"Błąd przy tworzeniu roli użytkownika: {roleName}");
            }
        }
    }
}
