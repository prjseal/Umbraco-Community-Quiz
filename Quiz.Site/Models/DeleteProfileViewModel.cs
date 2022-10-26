using System.ComponentModel.DataAnnotations;

namespace Quiz.Site.Models
{
    public class DeleteProfileViewModel
    {
        [Display(Name = "Avatar")]
        public IFormFile? Avatar { get; set; }

        public string? AvatarUrl { get; set; }

        public string? Name { get; set; }
        
        [Range(typeof(bool), "true", "true", ErrorMessage = "Confirm your account deletion!")]
        public bool ConfirmedDeletion { get; set; }
    }
}
