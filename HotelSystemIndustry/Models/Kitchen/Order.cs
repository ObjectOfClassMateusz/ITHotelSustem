using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

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


    [PrimaryKey("OrderId", "ProductId")]
    public class OrderProduct
    {
        [Required]
        public Guid OrderId { get; set; }

        public virtual Order? Order { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        public virtual KitchenProduct? Product { get; set; }


        public int Count { get; set; }
    }


    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime SubmissionTime { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? RealisedTime { get; set; } = null;


        [Required]
        public Guid TypeId { get; set; }

        public virtual OrderType? Type { get; set; }



        [Required, MaxLength(100, ErrorMessage = "Opis miejsca docelowego dostawy jest zbyt długi")]
        public required string DeliveryDestination { get; set; }

        public virtual ICollection<OrderProduct>? Products { get; set; }
    }

}