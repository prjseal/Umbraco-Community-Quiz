using Quiz.Site.Validation;
using System.ComponentModel.DataAnnotations;

namespace Quiz.Site.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

       [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "You must enter a valid email address")]
        [Required(ErrorMessage = "You must enter your email address")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "You must enter a password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,256}$", ErrorMessage = "Password must be at least 8 characters long, have 1 uppercase, 1 lowercase, 1 number and 1 special character.")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "You must confirm the password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Yes, I give permission to store and process my data")]
        [Required(ErrorMessage = "You must give consent to us storing your details before you can create an account")]
        [MustBeTrue(ErrorMessage = "You must give consent to us storing your details before you can create an account")]
        public bool Consent { get; set; }

        // ReSharper disable once InconsistentNaming
        public string hCaptchaSiteKey { get; set; } = string.Empty;
    }
}