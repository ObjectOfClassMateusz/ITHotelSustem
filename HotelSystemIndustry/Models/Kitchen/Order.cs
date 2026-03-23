using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Kitchen
{

    public class OrderType : DictionaryPrototype
    {
        /* Wartościami mogą być np.:
        * - TABLE_ORDER,
        * - ROOM_ORDER,
        * - TAKEAWAY_ORDER
        */
    }


    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime SubmissionTime { get; set; }


        public required Guid TypeId { get; set; }

        public virtual required OrderType Type { get; set; }


        [Required, MaxLength(100, ErrorMessage = "Opis miejsca docelowego dostawy jest zbyt długi")]
        public required string DeliveryDestination { get; set; }

        public virtual ICollection<KitchenProduct>? Products { get; set; }
    }

}