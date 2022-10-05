using Microsoft.AspNetCore.Mvc;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "DashboardNavigation")]
    public class DashboardNavigationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
