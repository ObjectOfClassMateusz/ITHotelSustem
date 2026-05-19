using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Infrastructure.DTO
{
    public class CreateGuestDTO
    {
        public Guid HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Wpisz imię")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Wpisz nazwisko")]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Nieprawidłowy e-mail")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Nieprawidłowy numer telefonu")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
