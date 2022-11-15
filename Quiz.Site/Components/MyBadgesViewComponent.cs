using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "MyBadges")]
    public class MyBadgesViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ProfileViewModel profileModel)
        {
            var model = new MyBadgesViewModel();
            model.Badges = profileModel.Badges.Reverse();
            return View(model);
        }
    }
}
