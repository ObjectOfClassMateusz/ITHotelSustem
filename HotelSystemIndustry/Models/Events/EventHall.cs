using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Events
{
    
    public class EventHall
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public required string Name { get; set; }

        public uint NumMaxGuests { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal ReservationPrice { get; set; }

        public ICollection<EquipmentInstance>? Equipment { get; set; }
    }

}