using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "ProfileResults")]
    public class ProfileResultsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ProfileResultsViewModel model)
        {
            return View(model);
        }
    }
}
