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
            await AddRoleAsync(roleManager, "Customer");
            await AddRoleAsync(roleManager, "HotelEmployee");
            await AddRoleAsync(roleManager, "KitchenEmployee");
            await AddRoleAsync(roleManager, "MaintainanceEmployee");
            await AddRoleAsync(roleManager, "TradingEmployee");
            await AddRoleAsync(roleManager, "RecreationEmployee");

            string adminEmail = "admin@admin.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new User
                {
                    FullName = "Właściciel Hotelu",
                    UserName = adminEmail,
                    NormalizedUserName = adminEmail.ToUpper(),
                    Email = adminEmail,
                    NormalizedEmail = adminEmail.ToUpper(),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await userManager.CreateAsync(adminUser, "admin123$N");
                if (result.Succeeded)
                {
                    logger.LogInformation("Nadawanie roli Admin administratorowi");
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    logger.LogError("Błąd przy tworzeniu użytkownika administratora {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Wystąpił błąd przy seedowaniu bazy danych");
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
