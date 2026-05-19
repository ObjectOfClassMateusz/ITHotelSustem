using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Events
{
    
    public class EquipmentInstance
    {
        [Key]
        public Guid Id { get; set; }


        [Required]
        public Guid EquipmentId { get; set; }

        public virtual Equipment? Equipment { get; set; }


        [Required]
        public Guid EventHallId { get; set; }

        public virtual EventHall? EventHall { get; set; }

        
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString="{0:C2}")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal ReservationPrice { get; set; }
    }

}