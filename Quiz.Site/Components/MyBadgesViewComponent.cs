using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Security;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "MyBadges")]
    public class MyBadgesViewComponent : ViewComponent
    {
        private readonly IAccountService _accountService;

        public MyBadgesViewComponent(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public IViewComponentResult Invoke(MemberIdentityUser user)
        {
            var member = _accountService.GetMemberModelFromUser(user);
            var enrichedProfile = _accountService.GetEnrichedProfile(member);

            var model = new MyBadgesViewModel();
            model.Badges = enrichedProfile.Badges;
            return View(model);
        }
    }
}
