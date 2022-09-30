using Microsoft.AspNetCore.Mvc;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "MyBadges")]
    public class MyBadgesViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
