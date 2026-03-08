using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Events
{
    
    public class EquipmentInstance
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public required Equipment Equipment { get; set; }
        
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal ReservationPrice { get; set; }
    }

}