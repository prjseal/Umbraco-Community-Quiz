using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Security;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "ProfileResults")]
    public class ProfileResultsViewComponent : ViewComponent
    {
        private readonly IAccountService _accountService;
        private readonly IQuizResultRepository _quizResultRepository;
        private readonly IMemberManager _memberManager;

        public ProfileResultsViewComponent(IAccountService accountService, IQuizResultRepository quizResultRepository, IMemberManager memberManager)
        {
            _accountService = accountService;
            _quizResultRepository = quizResultRepository;
            _memberManager = memberManager;
        }

        public IViewComponentResult Invoke(MemberIdentityUser user)
        {
            var member = _accountService.GetMemberModelFromUser(user);

            var enrichedProfile = _accountService.GetEnrichedProfile(member);

            ProfileResultsViewModel model = new ProfileResultsViewModel();

            var playerRecord = _quizResultRepository.GetPlayerRecordByMemberId(member.Id);

            IPublishedContent memberContent = null;
            playerRecord.Badges = 0;
            if (member != null)
            {
                var badgeCount = 0;
                memberContent = _memberManager.AsPublishedMember(user);
                if (memberContent != null)
                {
                    playerRecord.Name = memberContent.Name;

                    if (memberContent.HasProperty("badges") && memberContent.HasValue("badges"))
                    {
                        var badges = memberContent.Value<IEnumerable<IPublishedContent>>("badges");
                        badgeCount = badges != null && badges.Any() ? badges.Count() : 0;
                    }
                }
                playerRecord.Badges = badgeCount;
            }

            model.Member = member;
            model.Profile = enrichedProfile;
            model.PlayerRecord = playerRecord;

            return View(model);
        }
    }
}
