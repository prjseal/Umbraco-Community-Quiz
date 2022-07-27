using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "Register")]
    public class RegisterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(RegisterViewModel model)
        {
            return View(model);
        }
    }
}
