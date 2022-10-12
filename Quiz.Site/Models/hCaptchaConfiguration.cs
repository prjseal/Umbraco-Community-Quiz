using System.ComponentModel.DataAnnotations;

namespace Quiz.Site.Models;

// ReSharper disable once InconsistentNaming
public class hCaptchaConfiguration
{
    [Required]
    public string SiteKey { get; set; } = string.Empty;
    [Required]
    public string SecretKey { get; set; } = string.Empty;
}
