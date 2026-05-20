using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.ViewModels.Kitchen
{

    public class NewOrderNewProductViewModel
    {
        public NewOrderViewModel Order { get; set; } = new();
        
        public Guid? NewProductId { get; set; } = null;

        [Range(1, 5)]
        public int? NewProductCount { get; set; } = null;
    }
}