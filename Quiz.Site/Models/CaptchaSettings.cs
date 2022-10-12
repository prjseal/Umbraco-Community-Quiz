using System.ComponentModel.DataAnnotations;

namespace Quiz.Site.Models;

public class CaptchaSettings
{
    public const string Captcha = "Captcha";

    [Required]
    public string BaseUrl { get; set; } = string.Empty;

    [Required]
    public string EndPoint { get; set; } = string.Empty;

    [Required]
    public string SecretKey { get; set; } = string.Empty;

    [Required]
    public string SiteKey { get; set; } = string.Empty;
}
