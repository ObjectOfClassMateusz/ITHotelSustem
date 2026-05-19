using System.ComponentModel;
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

    public interface IService {  }

    public class Reservation : IService
    {
        [Key]
        public Guid Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        public ReservationStatus Status { get; set; }

        [Required(ErrorMessage = "Brak dni rezerwacji")]
        [DisplayName("Liczba nocy")]
        public int NumberOfOvernightStays { get; set; }

        [Required, MaxLength(10)]
        public required string NIP { get; set; }
        public string? SpecialWishes { get; set; }

        public Guid RoomId { get; set; }
        [Required(ErrorMessage = "Wymagany pokój do rezerwacji")]
        public Room Room { get; set; } = null!;

        [Required(ErrorMessage = "Wymagane opłacenie")]
        public required Payment Payment { get; set; }

        public Invoice? Invoice { get; set; }

        [Required(ErrorMessage = "Brak gości rezerwujących")]
        public ICollection<Guest> Guests { get; set; } = new List<Guest>();
    }
}
