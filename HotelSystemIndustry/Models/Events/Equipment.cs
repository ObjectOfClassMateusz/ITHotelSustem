using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Events
{

    public class EquipmentType : DictionaryPrototype
    {
        /*
        * np.:
        * - SLIDE_PROJECTOR,
        * - HIFI_SPEAKER,
        * - WIFI_ROUTER
        */
    }
    
    public class Equipment
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(30, ErrorMessage = "Nazwa jest zbyt długa")]
        public required string Name { get; set; }


        public required Guid TypeId { get; set; }
        public virtual required EquipmentType Type { get; set; }
    }

}