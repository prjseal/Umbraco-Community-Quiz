using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "Login")]
    public class LoginViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(LoginViewModel model)
        {
            return View(model);
        }
    }
}
