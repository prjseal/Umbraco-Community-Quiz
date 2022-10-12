namespace Quiz.Site.Models
{
    public class LoginViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        // ReSharper disable once InconsistentNaming
        public string hCaptchaSiteKey { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
