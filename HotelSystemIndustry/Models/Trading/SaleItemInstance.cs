using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Trading
{
    
    public class SaleItemInstance
    {
        [Key]
        public Guid Id { get; set; }

        public required SaleItem Item { get; set; }

        public ShopMagazine? Magazine { get; set; }

        public required string Variant { get; set; }
        [Required]
        public uint Count { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? ExpireDate { get; set; } = null;
    }

}