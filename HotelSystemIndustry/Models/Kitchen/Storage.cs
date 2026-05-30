using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Kitchen
{
    public class Storage
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(30, ErrorMessage = "Nazwa jest zbyt długa")]
        public required string Name { get; set; }

        [Required, MaxLength(50, ErrorMessage = "Nazwa lokalizacji jest zbyt długa")]
        public required string Location { get; set; }


        [Required]
        public Guid HotelId { get; set; }

        public Hotel? Hotel { get; set; }


        public virtual ICollection<ArticleInstance>? Articles { get; set; }
    }
}