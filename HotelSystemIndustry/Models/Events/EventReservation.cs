using System.ComponentModel.DataAnnotations;
using HotelSystemIndustry.Models.Kitchen;

namespace HotelSystemIndustry.Models.Events
{

    public class EventReservationStatus : DictionaryPrototype
    {
        /* np.:
        * - DURING_NEGOTIATION,
        * - BOOKED,
        * - PREPARING_EVENT,
        * - HAPPENING_NOW,
        * - FINISHED
        */
    }

    public class EventType : DictionaryPrototype
    {
        /* np.:
        * - CONFERENCE,
        * - BANQUET,
        * - WEDDING,
        * - FUNERAL_WAKE,
        * - BAPTISM,
        * - BIRTHDAY,
        * - NAME_DAY,
        * - OTHER
        */
    }
    
    public class EventReservation
    {
        [Key]
        public Guid Id { get; set; }


        public required Guid StatusId { get; set; }

        [Required]
        public virtual required EventReservationStatus Status { get; set; }


        public required Guid EventTypeId { get; set; }
        
        [Required]
        public virtual required EventType EventType { get; set; }


        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }
        

        [Range(typeof(uint), "0", "500")]
        public uint NumRequiredStaff { get; set; }

        [Range(typeof(uint), "0", "500")]
        public uint NumGuests { get; set; }


        public virtual ICollection<EventHall>? Halls { get; set; }

        public virtual ICollection<EquipmentInstance>? Equipment { get; set; }

        public virtual ICollection<Room>? Rooms { get; set; }

        public virtual ICollection<KitchenProduct>? Food { get; set; }
    }

}