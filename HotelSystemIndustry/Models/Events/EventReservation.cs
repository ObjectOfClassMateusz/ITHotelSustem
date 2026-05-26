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


        [Required]
        public Guid StatusId { get; set; }

        public virtual EventReservationStatus? Status { get; set; }



        [Required]
        public Guid EventTypeId { get; set; }
        
        public virtual EventType? EventType { get; set; }


        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }
        

        [Range(typeof(uint), "0", "500")]
        public uint NumRequiredStaff { get; set; }

        [Range(typeof(uint), "0", "500")]
        public uint NumGuests { get; set; }


        public string AgreementDocumentPath { get; set; } = string.Empty;


        public virtual ICollection<EventHall>? Halls { get; set; }

        public virtual ICollection<EquipmentInstance>? Equipment { get; set; }
    }

}