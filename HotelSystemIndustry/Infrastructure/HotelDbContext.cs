using HotelSystemIndustry.Models;
using HotelSystemIndustry.Models.Events;
using HotelSystemIndustry.Models.Kitchen;
using HotelSystemIndustry.Models.Trading;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Infrastructure
{
    public class HotelDbContext : DbContext//Singleton
    {
        public HotelDbContext(DbContextOptions options) : base(options)
        {
            
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
                .HasMany(p => p.Rooms)
                .WithOne(p => p.Hotel)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<Storage>().HasMany(s => s.Articles).WithOne(x => x.Storage).OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<KitchenRecipe>().HasMany(i => i.Ingredients).WithOne(x => x.Recipe).HasForeignKey(x => x.RecipeId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<KitchenRecipeIngredient>().HasOne(a => a.Article).WithMany().HasForeignKey(x => x.ArticleId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShopMagazine>().HasMany(m => m.Items).WithOne(x => x.Magazine).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
