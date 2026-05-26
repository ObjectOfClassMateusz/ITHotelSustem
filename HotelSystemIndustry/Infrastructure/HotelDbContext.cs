using HotelSystemIndustry.Models;
using HotelSystemIndustry.Models.Events;
using HotelSystemIndustry.Models.HousekeepingMaintenance;
using HotelSystemIndustry.Models.Recreation;
using HotelSystemIndustry.Models.Kitchen;
using HotelSystemIndustry.Models.Trading;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection.Emit;

namespace HotelSystemIndustry.Infrastructure
{
    public class HotelDbContext : IdentityDbContext<User>//Singleton
    {
        protected readonly IConfiguration _configuration;
        public HotelDbContext(IConfiguration conf , DbContextOptions options) : base(options)
        {
            _configuration = conf;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            Console.WriteLine(_configuration.GetConnectionString("HotelConnection"));
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("HotelConnection"));

            //Add-Migration InitialCreate
            //Update-Database
        }
        public virtual DbSet<Hotel> Hotels { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; } = default!;
        public virtual DbSet<EmployeeProfile> EmployeeProfiles { get; set; }
        public virtual DbSet<Guest> Guests { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Phone> Phones { get; set; }
        public virtual DbSet<Raport> Raports { get; set; }
        public virtual DbSet<RaportPayment> RaportPayments { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }


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

        public virtual DbSet<PurchaseItem> PurchaseItems { get; set; }


        public virtual DbSet<EmployeeShift> EmployeeShifts { get; set; }

        public virtual DbSet<HousekeepingSupply> HousekeepingSupplies { get; set; }

        public virtual DbSet<LostAndFoundItem> LostAndFoundItems { get; set; }

        public virtual DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }

        public virtual DbSet<RoomCleaning> RoomCleanings { get; set; }

        public virtual DbSet<SupplyUsage> SupplyUsages { get; set; }


        public virtual DbSet<RecreationBooking> RecreationBookings { get; set; }

        public virtual DbSet<RecreationFacility> RecreationFacilities { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Storage>()
                .HasMany(s => s.Articles)
                .WithOne(x => x.Storage)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<KitchenRecipe>()
                .HasMany(i => i.Ingredients)
                .WithOne(x => x.Recipe)
                .HasForeignKey(x => x.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KitchenRecipeIngredient>()
                .HasOne(a => a.Article)
                .WithMany()
                .HasForeignKey(x => x.ArticleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShopMagazine>()
                .HasMany(m => m.Items)
                .WithOne(x => x.Magazine)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Equipment>()
                .HasOne(e => e.Type)
                .WithMany()
                .HasForeignKey(e => e.TypeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<EquipmentInstance>()
                .HasOne(ei => ei.Equipment)
                .WithMany()
                .HasForeignKey(ei => ei.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<EquipmentInstance>()
                .HasOne(ei => ei.EventHall)
                .WithMany(eh => eh.Equipment)
                .HasForeignKey(ei => ei.EventHallId);
            modelBuilder.Entity<EventReservation>()
                .HasOne(er => er.EventType)
                .WithMany()
                .HasForeignKey(er => er.EventTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<EventReservation>()
                .HasOne(er => er.Status)
                .WithMany()
                .HasForeignKey(er => er.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KitchenArticle>()
                .HasOne(ka => ka.Type)
                .WithMany()
                .HasForeignKey(ka => ka.TypeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ArticleInstance>()
                .HasOne(ai => ai.Article)
                .WithMany(ka => ka.Instances)
                .HasForeignKey(ai => ai.ArticleId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ArticleInstance>()
                .HasOne(ai => ai.Storage)
                .WithMany(s => s.Articles)
                .HasForeignKey(ai => ai.StorageId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KitchenRecipe>()
                .HasOne(kr => kr.OutcomeProduct)
                .WithMany()
                .HasForeignKey(kr => kr.OutcomeProductId);
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Type)
                .WithMany()
                .HasForeignKey(o => o.TypeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SaleItem>()
                .HasOne(si => si.Type)
                .WithMany()
                .HasForeignKey(si => si.TypeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SaleItemInstance>()
                .HasOne(sii => sii.Item)
                .WithMany()
                .HasForeignKey(sii => sii.ItemId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<EventReservation>()
                .HasMany(er => er.Halls)
                .WithMany(eh => eh.EventReservations);

            modelBuilder.Entity<EventReservation>()
                .HasMany(er => er.Equipment)
                .WithMany();


            modelBuilder.Entity<Order>()
                .HasMany(o => o.Products)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderProduct>()
                .HasOne(o => o.Product)
                .WithMany()
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Hotel>()
                .HasMany(h => h.Rooms)
                .WithOne(r => r.Hotel)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Restrict);

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
                .HasMany(r => r.FoundItems)
                .WithOne(fi => fi.Room)
                .HasForeignKey(fi => fi.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Room>()
                .HasMany(r => r.Cleanings)
                .WithOne(c => c.Room)
                .HasForeignKey(c => c.RoomId)
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
                .WithOne(p => (Reservation?)p.Service)
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
                .HasForeignKey(rb => rb.GuestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RecreationFacility>()
                .HasMany(rf => rf.Bookings)
                .WithOne(rb => rb.Facility)
                .HasForeignKey(rb => rb.FacilityId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<Storage>()
                .HasMany(s => s.Articles)
                .WithOne(a => a.Storage)
                .HasForeignKey(a => a.StorageId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<KitchenRecipe>()
                .HasMany(i => i.Ingredients)
                .WithOne(x => x.Recipe)
                .HasForeignKey(x => x.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<KitchenRecipeIngredient>()
                .HasOne(a => a.Article)
                .WithMany()
                .HasForeignKey(x => x.ArticleId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ShopMagazine>()
                .HasMany(m => m.Items)
                .WithOne(x => x.Magazine)
                .HasForeignKey(x => x.MagazineId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoomCleaning>()
                .HasOne(r => r.Room)
                .WithMany(r => r.Cleanings)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MaintenanceRequest>()
                .HasOne(m => m.Room)
                .WithMany(r => r.MaintenanceRequests)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.ShopPoint)
                .WithMany()
                .HasForeignKey(p => p.ShopPointId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Purchase>()
                .HasMany(p => p.Items)
                .WithOne(p => p.Purchase)
                .HasForeignKey(p => p.PurchaseId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PurchaseItem>()
                .HasOne(p => p.SaleItem)
                .WithMany()
                .HasForeignKey(p => p.SaleItemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
