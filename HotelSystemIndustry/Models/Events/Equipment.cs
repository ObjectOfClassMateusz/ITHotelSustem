using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Events
{

    public enum EquipmentType
    {
        SLIDE_PROJECTOR,
        HIFI_SPEAKER,
        WIFI_ROUTER
    }
    
    public class Equipment
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public required string Name { get; set; }

        public EquipmentType Type { get; set; }
    }

}