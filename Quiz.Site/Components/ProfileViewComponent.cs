using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;

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

        public IViewComponentResult Invoke(IMember member)
        {
            var memberModel = _accountService.GetMemberModelFromMember(member);

            var enrichedProfile = _accountService.GetEnrichedProfile(memberModel);

            return View(enrichedProfile);
        }
    }
}
