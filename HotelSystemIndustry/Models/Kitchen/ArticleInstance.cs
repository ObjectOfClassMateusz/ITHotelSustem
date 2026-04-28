using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Kitchen
{
    public class ArticleInstance
    {
        [Key]
        public Guid Id { get; set; }


        [Required]
        public Guid ArticleId { get; set; }
        public virtual KitchenArticle? Article { get; set; }


        [Required]
        public Guid StorageId { get; set; }
        public virtual Storage? Storage { get; set; }


        /*
        * Ułamek może być przydatny dla artykułów sypkich (kg) i cieczy (l).
        * Dla artykułów dyskretnych można przechować po prostu liczbę całkowitą.
        */
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Count { get; set; }
    }

}