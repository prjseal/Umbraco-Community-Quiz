using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "AllBadges")]
    public class AllBadgesViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(BadgeListPage badgeListPage)
        {
            return View(badgeListPage);
        }
    }
}
