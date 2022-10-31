using System.ComponentModel.DataAnnotations;

namespace Quiz.Site.Models
{
    public class ForgottenPasswordRequestViewModel
    {
        [Required(ErrorMessage = "You must provide the email address for your account in order to request a forgotten password link.")]
        [EmailAddress]
        public string Email {get;set;}
    }
}