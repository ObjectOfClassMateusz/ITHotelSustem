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
                Id = Guid.NewGuid(), Name = "Chicken meat", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "meat")!.Id
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Beef", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "meat")!.Id
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Pork", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "meat")!.Id
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Chickpea", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "loose")!.Id
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Italian wine", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "liquid")!.Id
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Olive oil", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "liquid")!.Id
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Beer", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "liquid")!.Id
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Powder of beer flavour", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "loose")!.Id
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Egg", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "packed")!.Id
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Salad", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "vegetable")!.Id
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Apple", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "fruit")!.Id
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Wheat", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "loose")!.Id
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Potato", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "vegetable")!.Id
            });
            context.KitchenArticles.Add(new KitchenArticle
            {
                Id = Guid.NewGuid(), Name = "Pack of nuddles", TypeId = context.KitchenArticleTypes.FirstOrDefault(t => t.Value == "packed")!.Id
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
                Count = 0.5m, Unit = ArticleUnit.Kg
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == chickenSoupId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Pack of nuddles")!.Id,
                Count = 1, Unit = ArticleUnit.Pieces
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == chickenSoupId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Olive oil")!.Id,
                Count = 0.1m, Unit = ArticleUnit.Liters
            });

            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == porkchopId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Beef")!.Id,
                Count = 1.5m, Unit = ArticleUnit.Kg
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == porkchopId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Potato")!.Id,
                Count = 0.5m, Unit = ArticleUnit.Kg
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == porkchopId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Olive oil")!.Id,
                Count = 0.1m, Unit = ArticleUnit.Liters
            });

            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == chickpeaSaladId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Chickpea")!.Id,
                Count = 0.4m, Unit = ArticleUnit.Kg
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == chickpeaSaladId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Salad")!.Id,
                Count = 1, Unit = ArticleUnit.Pieces
            });

            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == burgerId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Beef")!.Id,
                Count = 0.4m, Unit = ArticleUnit.Kg
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == burgerId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Salad")!.Id,
                Count = 1, Unit = ArticleUnit.Pieces
            });

            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == friesId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Potato")!.Id,
                Count = 0.5m, Unit = ArticleUnit.Kg
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == friesId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Olive oil")!.Id,
                Count = 0.1m, Unit = ArticleUnit.Liters
            });

            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                RecipeId = context.KitchenRecipes.FirstOrDefault(r => r.OutcomeProductId == wineId)!.Id,
                ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Italian wine")!.Id,
                Count = 0.25m, Unit = ArticleUnit.Liters
            });

            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                 RecipeId = pancakes0RecipeId,
                 ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Egg")!.Id,
                 Count = 2, Unit = ArticleUnit.Pieces
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                 RecipeId = pancakes0RecipeId,
                 ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Wheat")!.Id,
                 Count = 0.5m, Unit = ArticleUnit.Kg
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                 RecipeId = pancakes0RecipeId,
                 ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Beer")!.Id,
                 Count = 0.1m, Unit = ArticleUnit.Liters
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                 RecipeId = pancakes0RecipeId,
                 ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Olive oil")!.Id,
                 Count = 0.1m, Unit = ArticleUnit.Liters
            });

            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                 RecipeId = pancakes1RecipeId,
                 ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Egg")!.Id,
                 Count = 2, Unit = ArticleUnit.Pieces
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                 RecipeId = pancakes1RecipeId,
                 ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Wheat")!.Id,
                 Count = 0.5m, Unit = ArticleUnit.Kg
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                 RecipeId = pancakes1RecipeId,
                 ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Powder of beer flavour")!.Id,
                 Count = 0.1m, Unit = ArticleUnit.Kg
            });
            context.KitchenRecipeIngredients.Add(new KitchenRecipeIngredient
            {
                 RecipeId = pancakes1RecipeId,
                 ArticleId = context.KitchenArticles.FirstOrDefault(a => a.Name == "Olive oil")!.Id,
                 Count = 0.1m, Unit = ArticleUnit.Liters
            });

            await context.SaveChangesAsync();
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
