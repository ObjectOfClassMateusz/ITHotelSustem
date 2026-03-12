using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Kitchen
{

    public class ArticleInstance
    {
        [Key]
        public Guid Id { get; set; }

        public Guid StorageId { get; set; }
        public virtual required Storage Storage { get; set; }

        public virtual required KitchenArticle Article { get; set; }

        /*
        * Ułamek może być przydatny dla artykułów sypkich (kg) i cieczy (l).
        * Dla artykułów dyskretnych można przechować po prostu liczbę całkowitą.
        */
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Count { get; set; }
    }

}