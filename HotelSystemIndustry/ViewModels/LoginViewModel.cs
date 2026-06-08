
using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please fill in your email address!")]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please fill in your password!")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Display(Name = "Zapamiętaj mnie")]
        public bool RememberMe { get; set; }
    }
}
