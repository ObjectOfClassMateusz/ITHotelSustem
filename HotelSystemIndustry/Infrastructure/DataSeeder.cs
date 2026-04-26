using HotelSystemIndustry.Models.Events;
using HotelSystemIndustry.Models.Kitchen;
using HotelSystemIndustry.Models.Trading;
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

            await SeedRolesAndUsers(roleManager, userManager);

            await SeedEvents(context);
            await SeedKitchen(context);
            await SeedTrading(context);

            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Wystąpił błąd przy seedowaniu bazy danych");
        }
    }


    private static async Task SeedRolesAndUsers(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
    {
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


    private static async Task SeedEvents(HotelDbContext context)
    {
        if (context.EquipmentTypes.Count() == 0)
        {
            context.EquipmentTypes.Add(new EquipmentType
            {
                Id = Guid.NewGuid(), Name = "Video projector", Value = "video-projector", IsActive = true
            });
            context.EquipmentTypes.Add(new EquipmentType
            {
                Id = Guid.NewGuid(), Name = "Hifi speaker", Value = "hifi-speaker", IsActive = true
            });
            context.EquipmentTypes.Add(new EquipmentType
            {
                Id = Guid.NewGuid(), Name = "Temperature controller", Value = "temperature-controller", IsActive = true
            });
            context.EquipmentTypes.Add(new EquipmentType
            {
                Id = Guid.NewGuid(), Name = "Microphone", Value = "microphone", IsActive = true
            });
        }


        if (context.EventReservationStatuses.Count() == 0)
        {
            context.EventReservationStatuses.Add(new EventReservationStatus
            {
                Id = Guid.NewGuid(), Name = "During negotiation", Value = "during-negotiation", IsActive = true
            });
            context.EventReservationStatuses.Add(new EventReservationStatus
            {
                Id = Guid.NewGuid(), Name = "Booked", Value = "booked", IsActive = true
            });
            context.EventReservationStatuses.Add(new EventReservationStatus
            {
                Id = Guid.NewGuid(), Name = "Preparing event", Value = "preparing-event", IsActive = true
            });
            context.EventReservationStatuses.Add(new EventReservationStatus
            {
                Id = Guid.NewGuid(), Name = "Happening now", Value = "happening-now", IsActive = true
            });
            context.EventReservationStatuses.Add(new EventReservationStatus
            {
                Id = Guid.NewGuid(), Name = "Finished", Value = "finished", IsActive = true
            });
        }

        if (context.EventTypes.Count() == 0)
        {
            context.EventTypes.Add(new EventType
            {
                Id = Guid.NewGuid(), Name = "Conference", Value = "conference", IsActive = true
            });
            context.EventTypes.Add(new EventType
            {
                Id = Guid.NewGuid(), Name = "Banquet", Value = "banquet", IsActive = true
            });
            context.EventTypes.Add(new EventType
            {
                Id = Guid.NewGuid(), Name = "Wedding", Value = "wedding", IsActive = true
            });
            context.EventTypes.Add(new EventType
            {
                Id = Guid.NewGuid(), Name = "Funeral wake", Value = "funeral-wake", IsActive = true
            });
            context.EventTypes.Add(new EventType
            {
                Id = Guid.NewGuid(), Name = "Baptism", Value = "baptism", IsActive = true
            });
            context.EventTypes.Add(new EventType
            {
                Id = Guid.NewGuid(), Name = "Birthday", Value = "birthday", IsActive = true
            });
            context.EventTypes.Add(new EventType
            {
                Id = Guid.NewGuid(), Name = "Name day", Value = "name-day", IsActive = true
            });
        }
    }

    private static async Task SeedKitchen(HotelDbContext context)
    {
        if (context.KitchenArticleTypes.Count() == 0)
        {
            context.KitchenArticleTypes.Add(new KitchenArticleType
            {
                Id = Guid.NewGuid(), Name = "Packed article", Value = "packed", IsActive = true
            });
            context.KitchenArticleTypes.Add(new KitchenArticleType
            {
                Id = Guid.NewGuid(), Name = "Loose article", Value = "loose", IsActive = true
            });
            context.KitchenArticleTypes.Add(new KitchenArticleType
            {
                Id = Guid.NewGuid(), Name = "Liquid", Value = "liquid", IsActive = true
            });
            context.KitchenArticleTypes.Add(new KitchenArticleType
            {
                Id = Guid.NewGuid(), Name = "Vegetable", Value = "vegetable", IsActive = true
            });
            context.KitchenArticleTypes.Add(new KitchenArticleType
            {
                Id = Guid.NewGuid(), Name = "Fruit", Value = "fruit", IsActive = true
            });
            context.KitchenArticleTypes.Add(new KitchenArticleType
            {
                Id = Guid.NewGuid(), Name = "Meat", Value = "meat", IsActive = true
            });
            context.KitchenArticleTypes.Add(new KitchenArticleType
            {
                Id = Guid.NewGuid(), Name = "Mushroom", Value = "mushroom", IsActive = true
            });
        }

        if (context.KitchenOrderTypes.Count() == 0)
        {
            context.KitchenOrderTypes.Add(new OrderType
            {
                Id = Guid.NewGuid(), Name = "Table order", Value = "table-order", IsActive = true
            });
            context.KitchenOrderTypes.Add(new OrderType
            {
                Id = Guid.NewGuid(), Name = "Room order", Value = "room-order", IsActive = true
            });
            context.KitchenOrderTypes.Add(new OrderType
            {
                Id = Guid.NewGuid(), Name = "Takeaway order", Value = "takeaway-order", IsActive = true
            });
        }

        if (context.KitchenProducts.Count() == 0)
        {
            context.KitchenProducts.Add(new KitchenProduct
            {
                Id = Guid.NewGuid(), Name = "Chicken Soup", ContainsAlcohol = false, Price = 9.0m
            });
            context.KitchenProducts.Add(new KitchenProduct
            {
                Id = Guid.NewGuid(), Name = "Pork chop", ContainsAlcohol = false, Price = 23.0m
            });
            context.KitchenProducts.Add(new KitchenProduct
            {
                Id = Guid.NewGuid(), Name = "Chickpea Salad", ContainsAlcohol = false, Price = 14.0m
            });
            context.KitchenProducts.Add(new KitchenProduct
            {
                Id = Guid.NewGuid(), Name = "Burger", ContainsAlcohol = false, Price = 19.99m
            });
            context.KitchenProducts.Add(new KitchenProduct
            {
                Id = Guid.NewGuid(), Name = "Fries", ContainsAlcohol = false, Price = 9.99m
            });
            context.KitchenProducts.Add(new KitchenProduct
            {
                Id = Guid.NewGuid(), Name = "Water", ContainsAlcohol = false, Price = 5.99m
            });
            context.KitchenProducts.Add(new KitchenProduct
            {
                Id = Guid.NewGuid(), Name = "Wine", ContainsAlcohol = true, Price = 22.99m
            });
        }
    }

    private static async Task SeedTrading(HotelDbContext context)
    {
        if (context.SaleItemTypes.Count() == 0)
        {
            context.SaleItemTypes.Add(new SaleItemType
            {
                Id = Guid.NewGuid(), Name = "To buy", Value = "to-buy", IsActive = true
            });
            context.SaleItemTypes.Add(new SaleItemType
            {
                Id = Guid.NewGuid(), Name = "For daily lease", Value = "for-daily-lease", IsActive = true
            });
            context.SaleItemTypes.Add(new SaleItemType
            {
                Id = Guid.NewGuid(), Name = "For monthly lease", Value = "for-monthly-lease", IsActive = true
            });
        }
    }
}
