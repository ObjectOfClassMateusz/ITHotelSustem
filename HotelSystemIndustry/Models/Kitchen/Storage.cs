using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Kitchen
{

    public class Storage
    {
        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Location { get; set; }


        public ICollection<ArticleInstance>? Articles { get; set; }
    }

}