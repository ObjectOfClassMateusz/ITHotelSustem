using System.ComponentModel.DataAnnotations;

namespace HotelSystemIndustry.ViewModels
{
    public class PasswordChangingViewModel
    {
        [Required(ErrorMessage = "Please enter your current password!")]
        [DataType(DataType.Password)]
        [Display(Name = "Obecne hasło")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Please choose your new password!")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        [Display(Name = "Nowe hasło")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Please repeat your new password for confirmation!")]
        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź nowe hasło")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmedNewPassword { get; set; }
    }
}
