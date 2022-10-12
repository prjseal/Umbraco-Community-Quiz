using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Quiz.Site.Models;

using IHostingEnvironment = Umbraco.Cms.Core.Hosting.IHostingEnvironment;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "Register")]
    public class RegisterViewComponent : ViewComponent
    {
        private readonly hCaptchaConfiguration _hCaptchaConfiguration;
        private readonly IHostingEnvironment _hHostingEnvironment;

        public RegisterViewComponent(IOptions<hCaptchaConfiguration> hCaptchaConfiguration, IHostingEnvironment hHostingEnvironment)
        {
            _hCaptchaConfiguration = hCaptchaConfiguration.Value;
            _hHostingEnvironment = hHostingEnvironment;
        }

        public IViewComponentResult Invoke(RegisterViewModel model)
        {
            if (model == null)
            {
                model = new RegisterViewModel();
            }

            if (!_hHostingEnvironment.IsDebugMode)
            {
                model.hCaptchaSiteKey = _hCaptchaConfiguration.SiteKey;
            }
            return View(model);
        }
    }
}
