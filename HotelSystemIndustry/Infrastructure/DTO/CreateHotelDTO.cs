using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.Infrastructure.DTO
{
    public class CreateHotelDTO
    {
        [Required]
        [DisplayName("Nazwa Hotelu")]
        [StringLength(29, ErrorMessage = "Nazwa przedsiębiorstwa może mieć maksymalnie 25 znaków.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [DisplayName("Opis")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DisplayName("Adres e-mail")]
        [EmailAddress(ErrorMessage = "Niepoprawny adres email.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DisplayName("Ulica i numer")]
        [RegularExpression(
        @"^ul\.\s?[A-ZĄĆĘŁŃÓŚŹŻa-ząćęłńóśźż0-9\s\-]+\s\d+[A-Za-z]?$",
        ErrorMessage = "Ulica w nieprawidłowym formacie.")]
        public string Street { get; set; } = string.Empty;

        [Required]
        [DisplayName("Miasto")]
        [RegularExpression(
         @"^[A-Za-z]+(?: [A-Za-z]+)*$",
        ErrorMessage = "Miasto w nieprawidłowym formacie.")]
        public string City { get; set; } = string.Empty;

        [Required]
        [DisplayName("Kod pocztowy")]
        [RegularExpression(
        @"^\d{2}-\d{3}$",
        ErrorMessage = "Kod pocztowy musi być w formacie XX-XXX.")]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [DisplayName("Kraj")]
        [RegularExpression(
         @"^[A-Za-z]+(?: [A-Za-z]+)*$",
        ErrorMessage = "Kraj w nieprawidłowym formacie.")]
        public string Country { get; set; } = string.Empty;

        public IList<string> PhoneNumbers { get; set; } = new List<string>();
    }
}
