using HotelSystemIndustry.Models;
using HotelSystemIndustry.Models.Events;
using HotelSystemIndustry.Models.Kitchen;
using HotelSystemIndustry.Models.Trading;
using HotelSystemIndustry.Models.HousekeepingMaintenance;
using HotelSystemIndustry.Models.Recreation;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Infrastructure
{
    public class HotelDbContext : DbContext//Singleton
    {
        protected readonly IConfiguration _configuration;
        public HotelDbContext(IConfiguration conf , DbContextOptions options) : base(options)
        {
            _configuration = conf;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("HotelConnection"));

            //Add-Migration InitialCreate
            //Update-Database
        }


        public virtual DbSet<Hotel> Hotels { get; set; }



        public virtual DbSet<EquipmentType> EquipmentTypes { get; set; }

        public virtual DbSet<Equipment> Equipment { get; set; }

        public virtual DbSet<EquipmentInstance> EquipmentInstances { get; set; }

        public virtual DbSet<EventHall> EventHalls { get; set; }

        public virtual DbSet<EventReservationStatus> EventReservationStatuses { get; set; }

        public virtual DbSet<EventType> EventTypes { get; set; }

        public virtual DbSet<EventReservation> EventReservations { get; set; }


        public virtual DbSet<Storage> KitchenStorages { get; set; }

        public virtual DbSet<KitchenArticleType> KitchenArticleTypes { get; set; }

        public virtual DbSet<KitchenArticle> KitchenArticles { get; set; }

        public virtual DbSet<ArticleInstance> KitchenArticleInstances { get; set; }

        public virtual DbSet<KitchenProduct> KitchenProducts { get; set; }

        public virtual DbSet<KitchenRecipeIngredient> KitchenRecipeIngredients { get; set; }

        public virtual DbSet<KitchenRecipe> KitchenRecipes { get; set; }

        public virtual DbSet<OrderType> KitchenOrderTypes { get; set; }

        public virtual DbSet<Order> KitchenOrders { get; set; }

        
        public virtual DbSet<ShopMagazine> ShopMagazines { get; set; }

        public virtual DbSet<SaleItemType> SaleItemTypes { get; set; }

        public virtual DbSet<SaleItem> SaleItems { get; set; }

        public virtual DbSet<SaleItemInstance> SaleItemInstances { get; set; }

        public virtual DbSet<ShopPoint> ShopPoints { get; set; }

        public virtual DbSet<Purchase> Purchases { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Hotel>()
                .HasMany(h => h.Rooms)
                .WithOne(r => r.Hotel)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<Storage>().HasMany(s => s.Articles).WithOne(x => x.Storage).OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<KitchenRecipe>().HasMany(i => i.Ingredients).WithOne(x => x.Recipe).HasForeignKey(x => x.RecipeId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KitchenRecipeIngredient>().HasOne(a => a.Article).WithMany().HasForeignKey(x => x.ArticleId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShopMagazine>().HasMany(m => m.Items).WithOne(x => x.Magazine).OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Hotel>()
                .HasMany(h => h.PhoneNumbers)
                .WithOne(p => p.Hotel)
                .HasForeignKey(p => p.HotelId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Hotel>()
                .HasOne(h => h.Address)
                .WithOne(a => a.Hotel)
                .HasForeignKey<Address>(a => a.HotelId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Room>()
                .HasMany(r => r.Reservations)
                .WithOne(r => r.Room)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Room>()
                .HasOne(r => r.foundItem)
                .WithOne(fi => fi.Room)
                .HasForeignKey<LostAndFoundItem>(fi => fi.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Room>()
                .HasMany(r => r.Cleanings)
                .WithOne(c => c.Room)
                .HasForeignKey(c => c.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<EventReservation>()
                .HasMany(e => e.Rooms)
                .WithOne(r => r.eventReservation)
                .HasForeignKey(r => r.EventReservationId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Room>()
                .HasMany(r => r.MaintenanceRequests)
                .WithOne(m => m.Room)
                .HasForeignKey(m =>  m.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Invoice)
                .WithOne(i => i.Reservation)
                .HasForeignKey<Invoice>(i => i.ReservationId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Payment)
                .WithOne(p => (Reservation)p.Service)
                .HasForeignKey<Payment>(p => p.ServiceId);

            modelBuilder.Entity<RaportPayment>()
            .HasKey(rp => new { rp.RaportId, rp.PaymentId });
            modelBuilder.Entity<RaportPayment>()
                .HasOne(rp => rp.Raport)
                .WithMany(r => r.RaportPayments)
                .HasForeignKey(rp => rp.RaportId);
            modelBuilder.Entity<RaportPayment>()
                .HasOne(rp => rp.Payment)
                .WithMany(p => p.RaportPayments)
                .HasForeignKey(rp => rp.PaymentId);
            modelBuilder.Entity<Reservation>()
                .HasMany(r => r.Guests)
                .WithMany(g => g.Reservations);

            modelBuilder.Entity<Guest>()
                .HasMany(g => g.RecreationBookings)
                .WithOne(rb => rb.Guest)
                .HasForeignKey(rb => rb.GuestId);
            modelBuilder.Entity<RecreationFacility>()
                .HasMany(rf => rf.Bookings)
                .WithOne(rb => rb.Facility)
                .HasForeignKey(rb => rb.FacilityId);
            /*modelBuilder.Entity<EventReservation>()
                .HasMany(er=>er.Halls)
                .WithOne(eh=>eh.Reservation)
                .HasForeignKey(eh=>eh.EventReservationId);*/
        }
    }
}
