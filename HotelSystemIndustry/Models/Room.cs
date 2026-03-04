using HotelSystemIndustry.Models.HousekeepingMaintenance;
using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Models
{
    public enum RoomType
    {
        Single = 0,
        Double = 1,
        Studio= 2,
        Apartment = 3
    }

    public class Room
    {
        [Key,Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Wpisz numer pokoju")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Wpisz piętro pokoju")]
        public int Floor { get; set; }

        [Range(1, 7),Required(ErrorMessage = "Wpisz rozmiar")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Wpisz cenę za noc")]
        public decimal BasePricePerNight { get; set; }
        public bool Renovation { get; set; }

        [Required(ErrorMessage = "Uzupełnij typ")]
        public RoomType RoomType { get; set; }

        public Guid HotelId { get; set; }
        [Required(ErrorMessage = "Wpisz hotel pokoju")]

        public Hotel Hotel { get; set; } = null!;
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        public ICollection<RoomCleaning> Cleanings { get; set; } = new List<RoomCleaning>(); //zaplanowane sprzątania
        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>(); //zgłoszenia konserwacyjne
    }
}
