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

        public ProfileResultsViewComponent(IAccountService accountService, IQuizResultRepository quizResultRepository)
        {
            _accountService = accountService;
            _quizResultRepository = quizResultRepository;
        }

        public IViewComponentResult Invoke(MemberIdentityUser user)
        {
            var member = _accountService.GetMemberModelFromUser(user);

            var enrichedProfile = _accountService.GetEnrichedProfile(member);

            ProfileResultsViewModel model = new ProfileResultsViewModel();

            var playerRecord = _quizResultRepository.GetPlayerRecordByMemberId(member.Id);

            if (playerRecord != null)
            {
                playerRecord.Badges = enrichedProfile.Badges.Count();
            }

            model.Profile = enrichedProfile;
            model.PlayerRecord = playerRecord;

            return View(model);
        }
    }
}
