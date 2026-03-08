using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Trading
{
    
    public class SaleItemInstance
    {
        [Key]
        public Guid Id { get; set; }

        public virtual required SaleItem Item { get; set; }

        public virtual ShopMagazine? Magazine { get; set; }

        [Required, MaxLength(50, ErrorMessage = "Opis wariantu produktu jest zbyt długi")]
        public required string Variant { get; set; }

        [Required, Range(typeof(uint), "0", "10000")]
        public uint Count { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ExpireDate { get; set; } = null;
    }

}