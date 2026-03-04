using System.ComponentModel.DataAnnotations;
using HotelSystemIndustry.Models.Kitchen;

namespace HotelSystemIndustry.Models.Events
{

    public enum EventReservationStatus
    {
        DURING_NEGOTIATION,
        BOOKED,
        PREPARING_EVENT,
        HAPPENING_NOW,
        FINISHED
    }

    public enum EventType
    {
        CONFERENCE,
        BANQUET,
        WEDDING,
        FUNERAL_WAKE,
        BAPTISM,
        BIRTHDAY,
        NAME_DAY,
        OTHER
    }
    
    public class EventReservation
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public EventReservationStatus Status { get; set; }

        public EventType EventType { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }

        public uint NumRequiredStaff { get; set; }

        public uint NumGuests { get; set; }


        public ICollection<EventHall>? Halls { get; set; }

        public ICollection<EquipmentInstance>? Equipment { get; set; }

        public ICollection<Room>? Rooms { get; set; }

        public ICollection<KitchenProduct>? Food { get; set; }
    }

}