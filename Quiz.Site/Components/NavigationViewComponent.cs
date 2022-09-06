using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "Navigation")]
    public class NavigationViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
