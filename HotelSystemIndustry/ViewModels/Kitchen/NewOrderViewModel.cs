using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.ViewModels.Kitchen
{
    public class ProductAndNumber
    {
        public Guid ProductId { get; set; }

        public int Count { get; set; }
    }

    public class NewOrderViewModel
    {
        [Required]
        public Guid Type { get; set; }

        [Required, MaxLength(100, ErrorMessage = "The delivery destination description is too long!")]
        public string Destination { get; set; } = string.Empty;

        public IList<ProductAndNumber> Products { get; set; } = new List<ProductAndNumber>();
    }
}