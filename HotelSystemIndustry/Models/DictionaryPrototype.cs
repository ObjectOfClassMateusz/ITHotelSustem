using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models
{
    
    public class DictionaryPrototype
    {
        [Key]
        public Guid Id { get; set; }


        [Required, MaxLength(30, ErrorMessage = "Nazwa jest zbyt długa")]
        public required string Name { get; set; }

        [Required, MaxLength(30, ErrorMessage = "Wartość jest zbyt długa")]
        public required string Value { get; set; }

        public bool IsActive { get; set; }


        [MaxLength(100, ErrorMessage = "Opis jest zbyt długi")]
        public string Description { get; set; } = string.Empty;
    }

}
