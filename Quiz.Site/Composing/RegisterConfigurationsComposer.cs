using Quiz.Site.Models;
using Umbraco.Cms.Core.Composing;

namespace Quiz.Site.Composing;

public class RegisterConfigurationsComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.Configure<hCaptchaConfiguration>((IConfiguration)builder.Config.GetSection("hCaptcha"));
    }
}
