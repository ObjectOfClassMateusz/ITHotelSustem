using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Kitchen
{

    public class ArticleInstance
    {
        [Key]
        public Guid Id { get; set; }

        public required Storage Storage { get; set; }

        public required KitchenArticle Article { get; set; }

        /*
        * Ułamek może być przydatny dla artykułów sypkich (kg) i cieczy (l).
        * Dla artykułów dyskretnych można przechować po prostu liczbę całkowitą.
        */
        public decimal Count { get; set; }
    }

}