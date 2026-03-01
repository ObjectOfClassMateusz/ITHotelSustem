using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Events
{
    
    public class EquipmentInstance
    {
        [Key]
        public Guid Id { get; set; }

        public required Equipment Equipment { get; set; }

        public decimal ReservationPrice { get; set; }
    }

}