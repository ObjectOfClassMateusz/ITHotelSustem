using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Kitchen
{

    public class KitchenProduct
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(30, ErrorMessage = "Nazwa jest zbyt długa")]
        public required string Name { get; set; }

        public bool ContainsAlcohol { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString="{0:C2}")]
        [Range(typeof(decimal), "0", "10000")]
        public decimal Price { get; set; }
    }

}