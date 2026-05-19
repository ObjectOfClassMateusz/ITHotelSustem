using HotelSystemIndustry.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Infrastructure
{
    public static class SeedData
    {
        public static async Task InitializeAsync(HotelDbContext db)
        {
            // Wait for schema to exist before querying
            /*
             * var pendingMigrations = await db.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                return; // Migration hasn't run yet, skip seeding
            }

            if (await db.Hotels.AnyAsync()) return; // already seeded

            var addresses = new[]
            {
            Address.Create("32-600 Brzezinka", "12 Kalinińskiego", "Ełk", "Poland"),
            Address.Create("32-600 Brzezinka", "14 Kalinińskiego", "Ełk", "Poland")
            };

            await db.Addresses.AddRangeAsync(addresses);
            await db.SaveChangesAsync();
            */
        }
    }
}
