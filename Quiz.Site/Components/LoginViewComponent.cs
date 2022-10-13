using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Quiz.Site.Models;
using Umbraco.Cms.Core.Configuration.Models;
using IHostingEnvironment = Umbraco.Cms.Core.Hosting.IHostingEnvironment;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "Login")]
    public class LoginViewComponent : ViewComponent
    {
        private readonly hCaptchaConfiguration _hCaptchaConfiguration;
        private readonly IHostingEnvironment _hHostingEnvironment;

        public LoginViewComponent(IOptions<hCaptchaConfiguration> hCaptchaConfiguration, IHostingEnvironment hHostingEnvironment)
        {
            _hHostingEnvironment = hHostingEnvironment;
            _hCaptchaConfiguration = hCaptchaConfiguration.Value;
        }

        public IViewComponentResult Invoke(LoginViewModel model)
        {
            if (model == null)
            {
                model = new LoginViewModel();
            }

            if (!_hHostingEnvironment.IsDebugMode)
            {
                model.hCaptchaSiteKey = _hCaptchaConfiguration.SiteKey;
            }
            return View(model);
        }
    }
}
