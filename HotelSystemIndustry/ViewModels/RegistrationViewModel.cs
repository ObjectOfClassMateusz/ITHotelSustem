
using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.ViewModels
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "Please fill in your full name!")]
        [StringLength(40, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "Pełna nazwa użytkownika")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please fill in your email!")]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please choose your password!")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please repeat your password for confirmation!")]
        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmedPassword { get; set; }
    }
}
