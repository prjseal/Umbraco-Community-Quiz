using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = Name)]
    public class ResetPasswordViewComponent : ViewComponent
    {        
        public const string Name = "ResetPassword";

        public IViewComponentResult Invoke(PasswordResetFormModel model)
        {
            return View(model ?? NewPasswordResetFormModel());
        }
        
        private PasswordResetFormModel NewPasswordResetFormModel()
        {            
            return new PasswordResetFormModel{
                RequestRef = Guid.NewGuid()
            };
        }

    }
}
