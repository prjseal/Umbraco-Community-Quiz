using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Quiz.Site.Models;
using Quiz.Site.Services;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "Profile")]
    public class ProfileViewComponent : ViewComponent
    {
        private readonly IAccountService _accountService;

        public ProfileViewComponent(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public IViewComponentResult Invoke(MemberIdentityUser user)
        {
            var member = _accountService.GetMemberModelFromUser(user);

            var enrichedProfile = _accountService.GetEnrichedProfile(member);

            return View(enrichedProfile);
        }
    }
}
