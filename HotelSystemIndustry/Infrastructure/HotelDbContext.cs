using HotelSystemIndustry.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Infrastructure
{
    public class HotelDbContext : DbContext//Singleton
    {
        public HotelDbContext(DbContextOptions options) : base(options)
        {
            
        }
        public virtual DbSet<Hotel> Hotels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Hotel>()
                .HasMany(p => p.Rooms)
                .WithOne(p => p.Hotel)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
