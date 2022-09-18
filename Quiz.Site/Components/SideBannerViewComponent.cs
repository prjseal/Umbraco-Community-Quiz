using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "SideBanner")]
    public class SideBannerViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
