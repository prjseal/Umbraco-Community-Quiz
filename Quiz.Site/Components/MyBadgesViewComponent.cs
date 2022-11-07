using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "MyBadges")]
    public class MyBadgesViewComponent : ViewComponent
    {
        private readonly IAccountService _accountService;
        private readonly IMemberService _memberService;

        public MyBadgesViewComponent(IAccountService accountService, IMemberService memberService)
        {
            _accountService = accountService;
            _memberService = memberService;
        }

        public IViewComponentResult Invoke(MemberIdentityUser user)
        {
            var member = _memberService.GetByEmail(user.Email);
            var memberModel = _accountService.GetMemberModelFromMember(member);
            var enrichedProfile = _accountService.GetEnrichedProfile(memberModel);

            var model = new MyBadgesViewModel();
            model.Badges = enrichedProfile.Badges;
            return View(model);
        }
    }
}
