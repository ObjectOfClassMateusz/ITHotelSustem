using HotelSystemIndustry.Models;
using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Infrastructure.DTO
{
    public class AddRoomDTO
    {
        public Guid HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty; // tylko do wyświetlenia

        [Required(ErrorMessage = "Wpisz numer pokoju")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Wpisz piętro")]
        public int Floor { get; set; }

        [Range(1, 7, ErrorMessage = "Pojemność musi być między 1 a 7")]
        [Required(ErrorMessage = "Wpisz pojemność")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Wpisz cenę za noc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cena musi być większa od 0")]
        public string BasePricePerNight { get; set; }

        public bool Renovation { get; set; }

        [Required(ErrorMessage = "Wybierz typ pokoju")]
        public RoomType RoomType { get; set; }
    }
}
