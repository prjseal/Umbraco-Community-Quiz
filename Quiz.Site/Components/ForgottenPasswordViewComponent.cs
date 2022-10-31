using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "ForgottenPassword")]
    public class ForgottenPasswordViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ForgottenPasswordRequestViewModel model)
        {            
            return View(model ??  new ForgottenPasswordRequestViewModel());
        }
    }
}
