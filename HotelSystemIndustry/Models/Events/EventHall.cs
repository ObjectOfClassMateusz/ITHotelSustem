using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Events
{
    
    public class EventHall
    {
        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public uint NumMaxGuests { get; set; }

        public decimal ReservationPrice { get; set; }

        public ICollection<EquipmentInstance>? Equipment { get; set; }
    }

}