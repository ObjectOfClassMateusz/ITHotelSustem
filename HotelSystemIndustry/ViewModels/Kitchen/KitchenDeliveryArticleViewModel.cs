using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.ViewModels.Kitchen
{
    public class KitchenArticleDelivery
    {
        [Required, Display(Name="Dostarczony artykuł")]
        public Guid ArticleId { get; set; }

        [Required, Display(Name="Do miejsca przechowywania")]
        public Guid ToStorageId { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335"), Display(Name="Ilość do dodania")]
        public decimal AddCount { get; set; } = 1;
    }

    public class KitchenDeliveryArticleViewModel
    {
        public IList<KitchenArticleDelivery> DeliveredArticles { get; set; } = new List<KitchenArticleDelivery>();

        public KitchenArticleDelivery ToAdd { get; set; } = new KitchenArticleDelivery();
    }
}