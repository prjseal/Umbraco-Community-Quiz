using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Quiz.Site.Models
{
    public class PasswordResetFormModel
    {
        public Guid RequestRef { get; set; }
        public string Password { get; set; }
        [DisplayName("Password confirmation")]
        [Compare(nameof(Password), ErrorMessage = "Passwords must match")]
        public string PasswordConfirmation { get; set; }
    }
}
