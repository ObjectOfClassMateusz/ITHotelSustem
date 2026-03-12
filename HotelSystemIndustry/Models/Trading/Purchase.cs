using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Trading
{
    
    public class Purchase
    {
        [Key]
        public Guid Id { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime TransactionDate { get; set; }

        public Guid ShopPointId { get; set; }
        [Required]
        public virtual ShopPoint? ShopPoint { get; set; }

        public virtual ICollection<SaleItemInstance>? Items { get; set; }
    }

}