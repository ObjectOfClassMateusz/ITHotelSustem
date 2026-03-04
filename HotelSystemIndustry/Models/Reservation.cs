using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models
{
    public enum ReservationStatus
    {
        Pending = 0,
        Confirmed = 1,
        Cancelled = 2,
        Completed = 3
    }

    public class Reservation
    {
        [Key]
        public Guid Id { get; set; }

        public Guid RoomId { get; set; }
        [Required(ErrorMessage = "Wymagany pokój do rezerwacji")]
        public Room Room { get; set; } = null!;

        [Required(ErrorMessage = "Brak gości rezerwujących")]
        public ICollection<Guest> Guests { get; set; } = new List<Guest>();

        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        public ReservationStatus Status { get; set; }

        [Required(ErrorMessage = "Wymagane opłacenie")]
        public required Payment Payment { get; set; }

        [Required(ErrorMessage = "Brak dni rezerwacji")]
        public int NumberOfOvernightStays { get; set; }

        [Required]
        public required Address Address { get; set; }

        [MaxLength(10)]
        public string NIP {  get; set; }

        public String? SpecialWishes { get; set; }
    }
}
