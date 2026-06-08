using HotelSystemIndustry.Models;
using HotelSystemIndustry.Models.Events;
using HotelSystemIndustry.Models.Kitchen;
using HotelSystemIndustry.Models.Recreation;
using HotelSystemIndustry.Models.Trading;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
            await SeedHotel(context);
            await SeedEvents(context);
            await SeedKitchen(context);
            await SeedTrading(context);
            await SeedHousekeepingAndRecreation(context);

            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e.InnerException, "Wystąpił błąd przy seedowaniu bazy danych");
        }
    }


    private static async Task SeedRolesAndUsers(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
    {
        await AddRoleAsync(roleManager, "Admin");
        await AddRoleAsync(roleManager, "HotelEmployee");
        await AddRoleAsync(roleManager, "KitchenEmployee");
        await AddRoleAsync(roleManager, "MaintenanceEmployee");
        await AddRoleAsync(roleManager, "HousekeepingEmployee");
        await AddRoleAsync(roleManager, "TradingEmployee");
        await AddRoleAsync(roleManager, "RecreationEmployee");

        await AddUserAsync(userManager, "admin@admin.com", "Właściciel Hotelu", "admin123$N", "Admin");
        await AddUserAsync(userManager, "hotellady@hotel.com", "Anna Machelska", "admin123$N", "HotelEmployee");
        await AddUserAsync(userManager, "kitchenlady@kitchen.com", "Anna Niedzielska", "admin123$N", "KitchenEmployee");
        await AddUserAsync(userManager, "maintenanceguy@maintenance.com", "Marek Niedzielski", "admin123$N", "MaintenanceEmployee");
        await AddUserAsync(userManager, "housekeepinglady@housekeeping.com", "Zofia Kowalska", "admin123$N", "HousekeepingEmployee");
        await AddUserAsync(userManager, "tradingperson@trading.com", "Orestes Niedzielski", "admin123$N", "TradingEmployee");
        await AddUserAsync(userManager, "recreationguy@recreation.com", "Sławomir Niedzielski", "admin123$N", "RecreationEmployee");

        await AddUserAsync(userManager, "malwiech@wp.pl", "Małgorzata Wiech", "admin123$N", "HotelEmployee");
        var malwiech = await userManager.FindByEmailAsync("malwiech@wp.pl");
        if (malwiech != null)
        {
            await userManager.AddToRoleAsync(malwiech, "KitchenEmployee");
            await userManager.AddToRoleAsync(malwiech, "MaintenanceEmployee");
            await userManager.AddToRoleAsync(malwiech, "TradingEmployee");
            await userManager.AddToRoleAsync(malwiech, "RecreationEmployee");
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


    private static async Task SeedEvents(HotelDbContext context)
    {
        var hotel = await context.Hotels.FirstOrDefaultAsync(h => h.Name == "Hotel Alfa Dominicana");
        if (hotel == null)
            hotel = await context.Hotels.FirstOrDefaultAsync();
        if (hotel == null)
            throw new Exception($"Błąd przy seedowaniu wydarzeń: nie znaleziono żadnego hotelu!");
        

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

            await context.SaveChangesAsync();
        }

        if (context.Equipment.Count() == 0)
        {
            context.Equipment.Add(new Equipment
            {
                Id = Guid.NewGuid(), Name = "MinBieda Video Projector", TypeId = context.EquipmentTypes.FirstOrDefault(t => t.Value == "video-projector")!.Id
            });
            context.Equipment.Add(new Equipment
            {
                Id = Guid.NewGuid(), Name = "MaxPro Video Projector", TypeId = context.EquipmentTypes.FirstOrDefault(t => t.Value == "video-projector")!.Id
            });
            context.Equipment.Add(new Equipment
            {
                Id = Guid.NewGuid(), Name = "AvgJoe Video Projector", TypeId = context.EquipmentTypes.FirstOrDefault(t => t.Value == "video-projector")!.Id
            });
            context.Equipment.Add(new Equipment
            {
                Id = Guid.NewGuid(), Name = "Harmann-Kardon Speaker", TypeId = context.EquipmentTypes.FirstOrDefault(t => t.Value == "hifi-speaker")!.Id
            });
            context.Equipment.Add(new Equipment
            {
                Id = Guid.NewGuid(), Name = "JBL Speaker", TypeId = context.EquipmentTypes.FirstOrDefault(t => t.Value == "hifi-speaker")!.Id
            });
            context.Equipment.Add(new Equipment
            {
                Id = Guid.NewGuid(), Name = "Universal temperature controller", TypeId = context.EquipmentTypes.FirstOrDefault(t => t.Value == "temperature-controller")!.Id
            });
            context.Equipment.Add(new Equipment
            {
                Id = Guid.NewGuid(), Name = "Bluetooth microphone", TypeId = context.EquipmentTypes.FirstOrDefault(t => t.Value == "microphone")!.Id
            });
            context.Equipment.Add(new Equipment
            {
                Id = Guid.NewGuid(), Name = "Cable microphone", TypeId = context.EquipmentTypes.FirstOrDefault(t => t.Value == "microphone")!.Id
            });

            await context.SaveChangesAsync();
        }


        if (context.EventHalls.Count() == 0)
        {
            context.EventHalls.Add(new EventHall
            {
                Id = Guid.NewGuid(), Name = "Conference Hall", NumMaxGuests = 300, ReservationPrice = 200.0m, HotelId = hotel.Id, Hotel = hotel
            });
            context.EventHalls.Add(new EventHall
            {
                Id = Guid.NewGuid(), Name = "Banquette Hall", NumMaxGuests = 150, ReservationPrice = 150.0m, HotelId = hotel.Id, Hotel = hotel
            });
            context.EventHalls.Add(new EventHall
            {
                Id = Guid.NewGuid(), Name = "Main Assembly Hall", NumMaxGuests = 500, ReservationPrice = 300.0m, HotelId = hotel.Id, Hotel = hotel
            });

            await context.SaveChangesAsync();
        }


        if (context.EquipmentInstances.Count() == 0)
        {
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "MinBieda Video Projector")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Banquette Hall")!.Id, ReservationPrice = 10.0m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "MinBieda Video Projector")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Conference Hall")!.Id, ReservationPrice = 10.0m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "MaxPro Video Projector")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Conference Hall")!.Id, ReservationPrice = 60.0m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "AvgJoe Video Projector")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Main Assembly Hall")!.Id, ReservationPrice = 30.0m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "MaxPro Video Projector")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Main Assembly Hall")!.Id, ReservationPrice = 60.0m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "Harmann-Kardon Speaker")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Banquette Hall")!.Id, ReservationPrice = 50.0m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "Harmann-Kardon Speaker")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Banquette Hall")!.Id, ReservationPrice = 50.0m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "Harmann-Kardon Speaker")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Main Assembly Hall")!.Id, ReservationPrice = 50.0m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "Harmann-Kardon Speaker")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Main Assembly Hall")!.Id, ReservationPrice = 50.0m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "Harmann-Kardon Speaker")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Main Assembly Hall")!.Id, ReservationPrice = 60.0m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "Harmann-Kardon Speaker")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Main Assembly Hall")!.Id, ReservationPrice = 60.0m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "JBL Speaker")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Conference Hall")!.Id, ReservationPrice = 40.0m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "JBL Speaker")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Conference Hall")!.Id, ReservationPrice = 40.0m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "Universal temperature controller")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Conference Hall")!.Id, ReservationPrice = 15.0m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "Universal temperature controller")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Banquette Hall")!.Id, ReservationPrice = 25.0m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "Universal temperature controller")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Main Assembly Hall")!.Id, ReservationPrice = 20.0m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "Cable microphone")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Conference Hall")!.Id, ReservationPrice = 5.99m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "Cable microphone")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Main Assembly Hall")!.Id, ReservationPrice = 5.99m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "Bluetooth microphone")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Banquette Hall")!.Id, ReservationPrice = 7.99m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "Bluetooth microphone")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Main Assembly Hall")!.Id, ReservationPrice = 7.99m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "Bluetooth microphone")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Conference Hall")!.Id, ReservationPrice = 7.99m
            });
            context.EquipmentInstances.Add(new EquipmentInstance
            {
                Id = Guid.NewGuid(), EquipmentId = context.Equipment.FirstOrDefault(e => e.Name == "Bluetooth microphone")!.Id,
                EventHallId = context.EventHalls.FirstOrDefault(eh => eh.Name == "Conference Hall")!.Id, ReservationPrice = 7.99m
            });

            await context.SaveChangesAsync();
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

            await context.SaveChangesAsync();
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

            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedKitchen(HotelDbContext context)
    {
        var hotel = await context.Hotels.FirstOrDefaultAsync(h => h.Name == "Hotel Alfa Dominicana");
        if (hotel == null)
            hotel = await context.Hotels.FirstOrDefaultAsync();
        if (hotel == null)
            throw new Exception($"Błąd przy seedowaniu kuchni: nie znaleziono żadnego hotelu!");


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
                Id = Guid.NewGuid(), Name = "Discrete article", Value = "discrete", IsActive = true
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

            await context.SaveChangesAsync();
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

        if (context.KitchenArticles.Count() == 0)
        {
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Chicken meat", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "meat")!.Id, Unit = ArticleUnit.Kg
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Beef", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "meat")!.Id, Unit = ArticleUnit.Kg
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Pork", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "meat")!.Id, Unit = ArticleUnit.Kg
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Chickpea", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "loose")!.Id, Unit = ArticleUnit.Kg
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Italian wine", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "liquid")!.Id, Unit = ArticleUnit.Liters
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Olive oil", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "liquid")!.Id, Unit = ArticleUnit.Liters
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Beer", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "liquid")!.Id, Unit = ArticleUnit.Liters
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Powder of beer flavour", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "loose")!.Id, Unit = ArticleUnit.Kg
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Egg", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "discrete")!.Id, Unit = ArticleUnit.Pieces
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Salad", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "vegetable")!.Id, Unit = ArticleUnit.Pieces
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Apple", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "fruit")!.Id, Unit = ArticleUnit.Pieces
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Wheat", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "loose")!.Id, Unit = ArticleUnit.Kg
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Potato", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "vegetable")!.Id, Unit = ArticleUnit.Pieces
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Pack of nuddles", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "packed")!.Id, Unit = ArticleUnit.Pieces
            });

            await context.SaveChangesAsync();
        }

        if (context.KitchenStorages.Count() == 0)
        {
            context.KitchenStorages.Add(new Storage
            {
                Id = Guid.NewGuid(), Name = "Fridge", Location = "Kitchen", HotelId = hotel.Id, Hotel = hotel
            });
            context.KitchenStorages.Add(new Storage
            {
                Id = Guid.NewGuid(), Name = "Basement Storage", Location = "Hotel basement", HotelId = hotel.Id, Hotel = hotel
            });

            await context.SaveChangesAsync();
        }

        if (context.KitchenArticleInstances.Count() == 0)
        {
            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Chicken meat")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Fridge")!.Id,
                Count = 3
            });
            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Beef")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Fridge")!.Id,
                Count = 2
            });
            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Pork")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Fridge")!.Id,
                Count = 2
            });
            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Olive oil")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Fridge")!.Id,
                Count = 4
            });
            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Egg")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Fridge")!.Id,
                Count = 36
            });
            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Salad")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Fridge")!.Id,
                Count = 4
            });
            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Beer")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Fridge")!.Id,
                Count = 24
            });
            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Chickpea")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Fridge")!.Id,
                Count = 10
            });

            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Wheat")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Basement Storage")!.Id,
                Count = 40
            });
            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Egg")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Basement Storage")!.Id,
                Count = 100
            });
            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Italian wine")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Basement Storage")!.Id,
                Count = 25
            });
            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Pack of nuddles")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Basement Storage")!.Id,
                Count = 15
            });
            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Potato")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Basement Storage")!.Id,
                Count = 42
            });
            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Olive oil")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Basement Storage")!.Id,
                Count = 8
            });
            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Apple")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Basement Storage")!.Id,
                Count = 12
            });
            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Chickpea")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Basement Storage")!.Id,
                Count = 15
            });
            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Powder of beer flavour")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Basement Storage")!.Id,
                Count = 30
            });
            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Pork")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Basement Storage")!.Id,
                Count = 7
            });
            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Beef")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Basement Storage")!.Id,
                Count = 6
            });
            context.KitchenArticleInstances.Add(new ArticleInstance
            {
                Id = Guid.NewGuid(),
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Chicken meat")!.Id,
                StorageId = context.KitchenStorages.FirstOrDefault(s => s.Name == "Basement Storage")!.Id,
                Count = 22
            });

            await context.SaveChangesAsync();
        }

        if (context.KitchenProducts.Count() == 0 &&
            context.KitchenRecipes.Count() == 0)
        {
            Guid chickenSoupId = Guid.NewGuid();
            Guid porkchopId = Guid.NewGuid();
            Guid chickpeaSaladId = Guid.NewGuid();
            Guid burgerId = Guid.NewGuid();
            Guid friesId = Guid.NewGuid();
            Guid waterId = Guid.NewGuid();
            Guid wineId = Guid.NewGuid();
            Guid pancakesId = Guid.NewGuid();
            Guid pancakes0RecipeId = Guid.NewGuid();
            Guid pancakes1RecipeId = Guid.NewGuid();

            context.KitchenProducts.Add(new KitchenProduct
            {
                Id = chickenSoupId, Name = "Chicken Soup", ContainsAlcohol = false, Price = 9.0m
            });
            context.KitchenProducts.Add(new KitchenProduct
            {
                Id = porkchopId, Name = "Pork chop", ContainsAlcohol = false, Price = 23.0m
            });
            context.KitchenProducts.Add(new KitchenProduct
            {
                Id = chickpeaSaladId, Name = "Chickpea Salad", ContainsAlcohol = false, Price = 14.0m
            });
            context.KitchenProducts.Add(new KitchenProduct
            {
                Id = burgerId, Name = "Burger", ContainsAlcohol = false, Price = 19.99m
            });
            context.KitchenProducts.Add(new KitchenProduct
            {
                Id = friesId, Name = "Fries", ContainsAlcohol = false, Price = 9.99m
            });
            context.KitchenProducts.Add(new KitchenProduct
            {
                Id = waterId, Name = "Water", ContainsAlcohol = false, Price = 5.99m
            });
            context.KitchenProducts.Add(new KitchenProduct
            {
                Id = wineId, Name = "Wine", ContainsAlcohol = true, Price = 22.99m
            });
            context.KitchenProducts.Add(new KitchenProduct
            {
                Id = pancakesId, Name = "Pancakes with beer", ContainsAlcohol = true, Price = 14.99m
            });

            context.KitchenRecipes.Add(new KitchenRecipe
            {
                Id = Guid.NewGuid(), Content = "Wrzucić kurę do garnka i gotować przez 40 minut.", OutcomeProductId = chickenSoupId
            });
            context.KitchenRecipes.Add(new KitchenRecipe
            {
                Id = Guid.NewGuid(), Content = "Woźmie zimnioki, a obierze je. Woźmie umyje. Na ruszt wrzuci i upiecze, a miukkie budo. Podowat' z widłami do jezenia dla ślachcica.", OutcomeProductId = porkchopId
            });
            context.KitchenRecipes.Add(new KitchenRecipe
            {
                Id = Guid.NewGuid(), Content = "Niech kret ugotuje Ci kapustę. Zawijaj masę ziemną w liście i gotuj w garnku przez 2 godziny.", OutcomeProductId = chickpeaSaladId
            });
            context.KitchenRecipes.Add(new KitchenRecipe
            {
                Id = Guid.NewGuid(), Content = "Zamknąć w bułkę smażony kotlet.", OutcomeProductId = burgerId
            });
            context.KitchenRecipes.Add(new KitchenRecipe
            {
                Id = Guid.NewGuid(), Content = "Pokroić zimnioki. Zasmażać zanużone w oleju palmowym.", OutcomeProductId = friesId
            });
            context.KitchenRecipes.Add(new KitchenRecipe
            {
                Id = Guid.NewGuid(), Content = "Nalać wina do lampki.", OutcomeProductId = wineId
            });
            context.KitchenRecipes.Add(new KitchenRecipe
            {
                Id = pancakes0RecipeId, Content = "Smażyć ciasto na oleju z piwem", OutcomeProductId = pancakesId
            });
            context.KitchenRecipes.Add(new KitchenRecipe
            {
                Id = pancakes1RecipeId, Content = "Dodać sproszkowane piwo do ciasta. Dokładnie wymieszać. Smażyć pół godziny.", OutcomeProductId = pancakesId
            });


            await context.SaveChangesAsync();


            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == chickenSoupId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Chicken meat")!.Id,
                Count = 0.5m
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == chickenSoupId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Pack of nuddles")!.Id,
                Count = 1
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == chickenSoupId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Olive oil")!.Id,
                Count = 0.1m
            });

            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == porkchopId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Beef")!.Id,
                Count = 1.5m
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == porkchopId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Potato")!.Id,
                Count = 3
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == porkchopId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Olive oil")!.Id,
                Count = 0.1m
            });

            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == chickpeaSaladId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Chickpea")!.Id,
                Count = 0.4m
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == chickpeaSaladId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Salad")!.Id,
                Count = 1
            });

            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == burgerId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Beef")!.Id,
                Count = 0.4m
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == burgerId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Salad")!.Id,
                Count = 1
            });

            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == friesId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Potato")!.Id,
                Count = 3
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == friesId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Olive oil")!.Id,
                Count = 0.1m
            });

            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == wineId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Italian wine")!.Id,
                Count = 0.25m
            });

            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                 RecipeId = pancakes0RecipeId,
                 ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Egg")!.Id,
                 Count = 2
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                 RecipeId = pancakes0RecipeId,
                 ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Wheat")!.Id,
                 Count = 0.5m
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                 RecipeId = pancakes0RecipeId,
                 ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Beer")!.Id,
                 Count = 0.1m
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                 RecipeId = pancakes0RecipeId,
                 ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Olive oil")!.Id,
                 Count = 0.1m
            });

            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                 RecipeId = pancakes1RecipeId,
                 ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Egg")!.Id,
                 Count = 2
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                 RecipeId = pancakes1RecipeId,
                 ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Wheat")!.Id,
                 Count = 0.5m
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                 RecipeId = pancakes1RecipeId,
                 ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Powder of beer flavour")!.Id,
                 Count = 0.1m
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                 RecipeId = pancakes1RecipeId,
                 ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Olive oil")!.Id,
                 Count = 0.1m
            });

            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedTrading(HotelDbContext context)
    {
        var hotel = await context.Hotels.FirstOrDefaultAsync(h => h.Name == "Hotel Alfa Dominicana");
        if (hotel == null)
            hotel = await context.Hotels.FirstOrDefaultAsync();
        if (hotel == null)
            throw new Exception($"Błąd przy seedowaniu modułu handlu: nie znaleziono żadnego hotelu!");


        if (context.SaleItemTypes.Count() == 0)
        {
            context.SaleItemTypes.Add(new SaleItemType
            {
                Id = Guid.NewGuid(), Name = "To buy", Value = "to-buy", IsActive = true, IsForRent = false
            });
            context.SaleItemTypes.Add(new SaleItemType
            {
                Id = Guid.NewGuid(), Name = "For daily lease", Value = "for-daily-lease", IsActive = true, IsForRent = true
            });
            context.SaleItemTypes.Add(new SaleItemType
            {
                Id = Guid.NewGuid(), Name = "For monthly lease", Value = "for-monthly-lease", IsActive = true, IsForRent = true
            });

            await context.SaveChangesAsync();
        }

        if (context.ShopMagazines.Count() == 0)
        {
            context.ShopMagazines.Add(new ShopMagazine
            {
                Id = Guid.NewGuid(), Location = "Souvenir shop facilities", HotelId = hotel.Id, Hotel = hotel
            });
            context.ShopMagazines.Add(new ShopMagazine
            {
                Id = Guid.NewGuid(), Location = "Supermarket facilities", HotelId = hotel.Id, Hotel = hotel
            });
            context.ShopMagazines.Add(new ShopMagazine
            {
                Id = Guid.NewGuid(), Location = "Hotel basement", HotelId = hotel.Id, Hotel = hotel
            });
            context.ShopMagazines.Add(new ShopMagazine
            {
                Id = Guid.NewGuid(), Location = "Garage", HotelId = hotel.Id, Hotel = hotel
            });

            await context.SaveChangesAsync();
        }

        if (context.ShopPoints.Count() == 0)
        {
            context.ShopPoints.Add(new ShopPoint
            {
                Id = Guid.NewGuid(), Location = "Souvenir shop in hotel lobby", HotelId = hotel.Id, Hotel = hotel
            });
            context.ShopPoints.Add(new ShopPoint
            {
                Id = Guid.NewGuid(), Location = "Supermarket after the hall", HotelId = hotel.Id, Hotel = hotel
            });
            context.ShopPoints.Add(new ShopPoint
            {
                Id = Guid.NewGuid(), Location = "Kiosk near guest rooms", HotelId = hotel.Id, Hotel = hotel
            });

            await context.SaveChangesAsync();
        }

        if (context.SaleItems.Count() == 0)
        {
            var toBuyType = context.SaleItemTypes.Single(t => t.Value == "to-buy");
            var forDailyLeaseType = context.SaleItemTypes.Single(t => t.Value == "for-daily-lease");
            var forMonthlyLeaseType = context.SaleItemTypes.Single(t => t.Value == "for-monthly-lease");

            context.SaleItems.Add(new SaleItem
            {
                Id = Guid.NewGuid(), Name = "Bread", ContainsAlcohol = false, TypeId = toBuyType.Id, Type = toBuyType
            });
            context.SaleItems.Add(new SaleItem
            {
                Id = Guid.NewGuid(), Name = "Greek wine", ContainsAlcohol = true, TypeId = toBuyType.Id, Type = toBuyType
            });
            context.SaleItems.Add(new SaleItem
            {
                Id = Guid.NewGuid(), Name = "White wine", ContainsAlcohol = true, TypeId = toBuyType.Id, Type = toBuyType
            });
            context.SaleItems.Add(new SaleItem
            {
                Id = Guid.NewGuid(), Name = "Plums in chocolate", ContainsAlcohol = false, TypeId = toBuyType.Id, Type = toBuyType
            });
            context.SaleItems.Add(new SaleItem
            {
                Id = Guid.NewGuid(), Name = "Can of tuna", ContainsAlcohol = false, TypeId = toBuyType.Id, Type = toBuyType
            });
            context.SaleItems.Add(new SaleItem
            {
                Id = Guid.NewGuid(), Name = "Mini hotel model", ContainsAlcohol = false, TypeId = toBuyType.Id, Type = toBuyType
            });
            context.SaleItems.Add(new SaleItem
            {
                Id = Guid.NewGuid(), Name = "Hotel puzzle set", ContainsAlcohol = false, TypeId = toBuyType.Id, Type = toBuyType
            });
            context.SaleItems.Add(new SaleItem
            {
                Id = Guid.NewGuid(), Name = "Official pancakes set", ContainsAlcohol = true, TypeId = toBuyType.Id, Type = toBuyType
            });
            context.SaleItems.Add(new SaleItem
            {
                Id = Guid.NewGuid(), Name = "Tourist car", ContainsAlcohol = false, TypeId = forDailyLeaseType.Id, Type = forDailyLeaseType
            });
            context.SaleItems.Add(new SaleItem
            {
                Id = Guid.NewGuid(), Name = "Blu-ray movie rental", ContainsAlcohol = false, TypeId = forDailyLeaseType.Id, Type = forDailyLeaseType
            });
            context.SaleItems.Add(new SaleItem
            {
                Id = Guid.NewGuid(), Name = "Parking slot", ContainsAlcohol = false, TypeId = forDailyLeaseType.Id, Type = forDailyLeaseType
            });
            context.SaleItems.Add(new SaleItem
            {
                Id = Guid.NewGuid(), Name = "Monthly parking slot", ContainsAlcohol = false, TypeId = forMonthlyLeaseType.Id, Type = forMonthlyLeaseType
            });

            await context.SaveChangesAsync();
        }

        if (context.SaleItemInstances.Count() == 0)
        {
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "Normal bread", Count = 12, ExpireDate = DateTime.UtcNow.AddDays(10).Date, Price = 3.99m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Bread")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Supermarket facilities")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "Portobello", Count = 5, ExpireDate = DateTime.UtcNow.AddYears(10).Date, Price = 89.95m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Greek wine")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Supermarket facilities")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "Portobello", Count = 10, ExpireDate = DateTime.UtcNow.AddYears(10).Date, Price = 59.95m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "White wine")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Supermarket facilities")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "", Count = 36, ExpireDate = DateTime.UtcNow.AddMonths(5).Date, Price = 10.95m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Plums in chocolate")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Supermarket facilities")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "With vegetables", Count = 23, ExpireDate = DateTime.UtcNow.AddYears(4).Date, Price = 3.99m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Can of tuna")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Supermarket facilities")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "Deluxe", Count = 4, ExpireDate = null, Price = 60.25m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Mini hotel model")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Souvenir shop facilities")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "Exclusive", Count = 6, ExpireDate = null, Price = 35.99m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Hotel puzzle set")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Souvenir shop facilities")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "With some ingredients packaged", Count = 5, ExpireDate = DateTime.UtcNow.AddMonths(5).Date, Price = 20.15m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Official pancakes set")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Souvenir shop facilities")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "Fiat 126p", Count = 2, ExpireDate = null, Price = 100.0m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Tourist car")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Garage")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "Fiat 500", Count = 3, ExpireDate = null, Price = 500.0m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Tourist car")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Garage")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "VW Beetle", Count = 5, ExpireDate = null, Price = 300.0m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Tourist car")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Garage")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "Lody na patyku", Count = 1, ExpireDate = null, Price = 10.99m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Blu-ray movie rental")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Hotel basement")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "Melancholia", Count = 3, ExpireDate = null, Price = 10.99m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Blu-ray movie rental")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Hotel basement")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "Breaking Bad", Count = 1, ExpireDate = null, Price = 15.99m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Blu-ray movie rental")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Hotel basement")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "Heated Rivalry", Count = 2, ExpireDate = null, Price = 12.99m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Blu-ray movie rental")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Hotel basement")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "Slot #1", Count = 1, ExpireDate = null, Price = 29.99m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Parking slot")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Garage")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "Slot #2", Count = 1, ExpireDate = null, Price = 29.99m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Parking slot")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Garage")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "Slot #3", Count = 1, ExpireDate = null, Price = 29.99m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Parking slot")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Garage")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "Slot #4", Count = 1, ExpireDate = null, Price = 29.99m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Parking slot")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Garage")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "Monthly slot #1", Count = 1, ExpireDate = null, Price = 300.99m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Monthly parking slot")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Garage")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "Monthly slot #2", Count = 1, ExpireDate = null, Price = 300.99m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Monthly parking slot")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Garage")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "Monthly slot #3", Count = 1, ExpireDate = null, Price = 300.99m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Monthly parking slot")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Garage")!.Id,
            });
            context.SaleItemInstances.Add(new SaleItemInstance
            {
                Id = Guid.NewGuid(), Variant = "Monthly slot #4", Count = 1, ExpireDate = null, Price = 300.99m,
                ItemId = context.SaleItems.FirstOrDefault(si => si.Name == "Monthly parking slot")!.Id,
                MagazineId = context.ShopMagazines.FirstOrDefault(m => m.Location == "Garage")!.Id,
            });
        }
    }

    private static async Task SeedHotel(HotelDbContext context)
    {
        var hotel = new Hotel() 
        { 
            Id = Guid.NewGuid(),
            Name = "Hotel Alfa Dominicana",
            Description = "Hotel jest idealnie położony, zaledwie kilka kroków od Pałacu Branickich i jego barokowych ogrodów, co zapewnia łatwy dostęp do atrakcji miasta.",
            Email = "alfadominicana@tutanota.com"
        };
        var address = new Address()
        {
            Id = Guid.NewGuid(),
            Hotel = hotel,
            HotelId = hotel.Id,
            Street = "ul. Zwierzyniecka 14",
            Country = "Polska",
            PostalCode = "15-333",
            City = "Białystok"
        };
        hotel.Address = address;
        var phone = new Phone()
        { 
            HotelId = hotel.Id,
            Hotel = hotel,
            Id = Guid.NewGuid(),
            PhoneNumber = "+48 856 521 182"
        };
        hotel.PhoneNumbers.Add(phone);

        if (context.Hotels.Count() == 0)
        {
            context.Hotels.Add(hotel);
            await context.SaveChangesAsync();
        }
        if (context.Addresses.Count() == 0) 
        {
            context.Addresses.Add(address);
            await context.SaveChangesAsync();
        }
        if (context.Phones.Count() == 0)
        {
            context.Phones.Add(phone);
            await context.SaveChangesAsync();
        }
    }
    private static async Task SeedHousekeepingAndRecreation(HotelDbContext context)
    {
        var hotel = await context.Hotels.FirstOrDefaultAsync(h => h.Name == "Hotel Alfa Dominicana");
        if (hotel == null)
            hotel = await context.Hotels.FirstOrDefaultAsync();
        if (hotel == null)
            throw new Exception("Błąd przy seedowaniu: nie znaleziono żadnego hotelu!");

        if (!await context.Guests.AnyAsync())
        {
            context.Guests.Add(new Guest
            {
                Id = Guid.NewGuid(),
                FirstName = "Jan",
                LastName = "Kowalski",
                HotelId = hotel.Id
            });
            context.Guests.Add(new Guest
            {
                Id = Guid.NewGuid(),
                FirstName = "Maria",
                LastName = "Nowak",
                HotelId = hotel.Id
            });

            await context.SaveChangesAsync();
        }

        if (!await context.RecreationFacilities.AnyAsync())
        {
            context.RecreationFacilities.Add(new RecreationFacility
            {
                Id = Guid.NewGuid(),
                Name = "Swimming Pool",
                Description = "Outdoor swimming pool",
                MaxCapacity = 20,
                PricePerHour = 15.00m,
                HotelId = hotel.Id
            });
            context.RecreationFacilities.Add(new RecreationFacility
            {
                Id = Guid.NewGuid(),
                Name = "Tennis Court",
                Description = "Indoor tennis court",
                MaxCapacity = 4,
                PricePerHour = 30.00m,
                HotelId = hotel.Id
            });

            await context.SaveChangesAsync();
        }
    }
}
