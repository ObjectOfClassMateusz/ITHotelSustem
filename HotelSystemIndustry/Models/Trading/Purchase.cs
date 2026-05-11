using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace HotelSystemIndustry.Models.Trading
{

    [PrimaryKey("PurchaseId", "SaleItemId")]
    public class PurchaseItem
    {
        [Required]
        public Guid PurchaseId { get; set; }

        public virtual Purchase? Purchase { get; set; }

        [Required]
        public Guid SaleItemId { get; set; }

        public virtual SaleItem? SaleItem { get; set; }


        public uint Count { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal UnitPrice { get; set; }

        public string Variant { get; set; } = string.Empty;
    }

    
    public class Purchase
    {
        [Key]
        public Guid Id { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime TransactionDate { get; set; }

        public Guid? ShopPointId { get; set; }
        
        public virtual ShopPoint? ShopPoint { get; set; }

        public virtual ICollection<PurchaseItem>? Items { get; set; }
    }

}
