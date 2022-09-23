using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Controllers
{
    public class EditProfilePageController : RenderController
    {
        public EditProfilePageController(ILogger<ProfilePageController> logger, ICompositeViewEngine compositeViewEngine, IUmbracoContextAccessor umbracoContextAccessor)
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
        }

        public override IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                var loginPage = CurrentPage.AncestorOrSelf<HomePage>().FirstChildOfType(LoginPage.ModelTypeAlias);
                return Redirect(loginPage?.Url() ?? "/");
            }
            return CurrentTemplate(CurrentPage);
        }
    }
}