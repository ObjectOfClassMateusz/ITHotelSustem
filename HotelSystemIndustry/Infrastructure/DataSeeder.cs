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


            string hotelEmail = "hotellady@hotel.com";
            if (await userManager.FindByEmailAsync(hotelEmail) == null)
            {
                var hotelUser = new User
                {
                    FullName = "Anna Machelska",
                    UserName = hotelEmail,
                    NormalizedUserName = hotelEmail.ToUpper(),
                    Email = hotelEmail,
                    NormalizedEmail = hotelEmail.ToUpper(),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await userManager.CreateAsync(hotelUser, "admin123$N");
                if (result.Succeeded)
                {
                    logger.LogInformation("Nadawanie roli użytkownika");
                    await userManager.AddToRoleAsync(hotelUser, "HotelEmployee");
                }
                else
                {
                    logger.LogError("Błąd przy tworzeniu użytkownika {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }


            string kitchenEmail = "kitchenlady@kitchen.com";
            if (await userManager.FindByEmailAsync(kitchenEmail) == null)
            {
                var kitchenUser = new User
                {
                    FullName = "Anna Niedzielska",
                    UserName = kitchenEmail,
                    NormalizedUserName = kitchenEmail.ToUpper(),
                    Email = kitchenEmail,
                    NormalizedEmail = kitchenEmail.ToUpper(),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await userManager.CreateAsync(kitchenUser, "admin123$N");
                if (result.Succeeded)
                {
                    logger.LogInformation("Nadawanie roli użytkownika");
                    await userManager.AddToRoleAsync(kitchenUser, "KitchenEmployee");
                }
                else
                {
                    logger.LogError("Błąd przy tworzeniu użytkownika {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }


            string maintainanceEmail = "maintainanceguy@maintainance.com";
            if (await userManager.FindByEmailAsync(maintainanceEmail) == null)
            {
                var maintainanceUser = new User
                {
                    FullName = "Marek Niedzielski",
                    UserName = maintainanceEmail,
                    NormalizedUserName = maintainanceEmail.ToUpper(),
                    Email = maintainanceEmail,
                    NormalizedEmail = maintainanceEmail.ToUpper(),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await userManager.CreateAsync(maintainanceUser, "admin123$N");
                if (result.Succeeded)
                {
                    logger.LogInformation("Nadawanie roli użytkownika");
                    await userManager.AddToRoleAsync(maintainanceUser, "MaintainanceEmployee");
                }
                else
                {
                    logger.LogError("Błąd przy tworzeniu użytkownika {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }


            string tradingEmail = "tradingperson@trading.com";
            if (await userManager.FindByEmailAsync(tradingEmail) == null)
            {
                var tradingUser = new User
                {
                    FullName = "Orestes Niedzielski",
                    UserName = tradingEmail,
                    NormalizedUserName = tradingEmail.ToUpper(),
                    Email = tradingEmail,
                    NormalizedEmail = tradingEmail.ToUpper(),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await userManager.CreateAsync(tradingUser, "admin123$N");
                if (result.Succeeded)
                {
                    logger.LogInformation("Nadawanie roli użytkownika");
                    await userManager.AddToRoleAsync(tradingUser, "TradingEmployee");
                }
                else
                {
                    logger.LogError("Błąd przy tworzeniu użytkownika {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }


            string recreationEmail = "recreationguy@recreation.com";
            if (await userManager.FindByEmailAsync(recreationEmail) == null)
            {
                var recreationUser = new User
                {
                    FullName = "Sławomir Niedzielski",
                    UserName = recreationEmail,
                    NormalizedUserName = recreationEmail.ToUpper(),
                    Email = recreationEmail,
                    NormalizedEmail = recreationEmail.ToUpper(),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await userManager.CreateAsync(recreationUser, "admin123$N");
                if (result.Succeeded)
                {
                    logger.LogInformation("Nadawanie roli użytkownika");
                    await userManager.AddToRoleAsync(recreationUser, "RecreationEmployee");
                }
                else
                {
                    logger.LogError("Błąd przy tworzeniu użytkownika {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
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
