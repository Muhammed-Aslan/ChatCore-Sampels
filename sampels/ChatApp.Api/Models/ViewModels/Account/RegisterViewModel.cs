using System.ComponentModel.DataAnnotations;

namespace ChatApp.Api.Models.Account
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "User Name")]
        [StringLength(50, ErrorMessage = "The User name must be between 6-50 characters long.", MinimumLength = 5)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The Password must be at least 6 characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Remember Me ?")]
        public bool RememberMe { get; set; }
    }
}
