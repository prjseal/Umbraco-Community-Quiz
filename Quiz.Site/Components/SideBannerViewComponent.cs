using Microsoft.AspNetCore.Mvc;

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
