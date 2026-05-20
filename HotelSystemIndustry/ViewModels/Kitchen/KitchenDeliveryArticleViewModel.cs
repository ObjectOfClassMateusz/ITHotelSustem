using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.ViewModels.Kitchen
{
    public class KitchenArticleDelivery
    {
        [Required, Display(Name="Delivered article")]
        public Guid ArticleId { get; set; }

        [Required, Display(Name="To storage")]
        public Guid ToStorageId { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335"), Display(Name="Count to add")]
        public decimal AddCount { get; set; } = 1;
    }

    public class KitchenDeliveryArticleViewModel
    {
        public IList<KitchenArticleDelivery> DeliveredArticles { get; set; } = new List<KitchenArticleDelivery>();

        public KitchenArticleDelivery ToAdd { get; set; } = new KitchenArticleDelivery();
    }
}