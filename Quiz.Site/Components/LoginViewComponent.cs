using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Quiz.Site.Models;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "Login")]
    public class LoginViewComponent : ViewComponent
    {
        private readonly hCaptchaConfiguration _hCaptchaConfiguration;

        public LoginViewComponent(IOptions<hCaptchaConfiguration> hCaptchaConfiguration)
        {
            _hCaptchaConfiguration = hCaptchaConfiguration.Value;
        }

        public IViewComponentResult Invoke(LoginViewModel model)
        {
            if (model == null)
            {
                model = new LoginViewModel();
            }
            model.hCaptchaSiteKey = _hCaptchaConfiguration.SiteKey;
            return View(model);
        }
    }
}
