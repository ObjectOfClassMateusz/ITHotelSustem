
using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please fill in your email address!")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please fill in your password!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
