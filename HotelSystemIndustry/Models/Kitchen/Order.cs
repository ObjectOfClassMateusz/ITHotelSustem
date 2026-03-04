using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models.Kitchen
{

    public enum OrderType
    {
        TABLE_ORDER,
        ROOM_ORDER,
        TAKEAWAY_ORDER
    }


    public class Order
    {
        [Key]
        public Guid Id { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime SubmissionTime { get; set; }
        public OrderType Type { get; set; }
        public required string DeliveryDestination { get; set; }
        public ICollection<KitchenProduct>? Products { get; set; }
    }

}