using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models
{
    public class Payment
    {
        [Key]
        public Guid Id { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Amount { get; set; }
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }
        [Required]
        public PaymentMethod Method { get; set; }
        [Required]
        public PaymentStatus Status { get; set; }


        public Guid ServiceId { get; set; }
        public IService? Service { get; set; }

        public ICollection<RaportPayment> RaportPayments { get; set; }
    }

    public enum PaymentMethod
    {
        Cash = 0,
        CreditCard = 1,
        BankTransfer = 2,
        OnlinePayment = 3
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Completed = 1,
        Failed = 2,
        Refunded = 3
    }
}
