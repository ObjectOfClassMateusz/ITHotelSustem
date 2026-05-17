using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Trading
{
    
    public class SaleItemInstance
    {
        [Key]
        public Guid Id { get; set; }


        [Required]
        public Guid ItemId { get; set; }

        public virtual SaleItem? Item { get; set; }

        [Required]
        public Guid MagazineId { get; set; }
        public virtual ShopMagazine? Magazine { get; set; }


        [Required, MaxLength(50, ErrorMessage = "Opis wariantu produktu jest zbyt długi")]
        public required string Variant { get; set; }

        [Required, Range(typeof(uint), "0", "10000")]
        public uint Count { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString="{0:C2}")]
        [Range(typeof(decimal), "0", "10000")]
        public decimal Price { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpireDate { get; set; } = null;
    }

}