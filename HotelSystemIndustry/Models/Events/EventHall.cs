using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Events
{
    public class EventHall
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(30, ErrorMessage = "Nazwa jest zbyt długa")]
        public required string Name { get; set; }

        [Range(typeof(uint), "0", "500")]
        public uint NumMaxGuests { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal ReservationPrice { get; set; }

        public virtual ICollection<EquipmentInstance>? Equipment { get; set; }
        public virtual ICollection<EventReservation>? EventReservations { get; set; }
    }
}