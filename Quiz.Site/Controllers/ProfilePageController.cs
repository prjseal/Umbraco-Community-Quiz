using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Quiz.Site.Models;
using Quiz.Site.Models.ContentModels;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Controllers
{
    public class ProfilePageController : RenderController
    {
        private readonly IMemberService _memberService;
        private readonly IAccountService _accountService;
        private readonly IMemberManager _memberManager;
        private readonly IQuizResultRepository _quizResultRepository;

        public ProfilePageController(ILogger<ProfilePageController> logger, ICompositeViewEngine compositeViewEngine, 
            IUmbracoContextAccessor umbracoContextAccessor, IMemberService memberService,
            IAccountService accountService, IMemberManager memberManager, IQuizResultRepository quizResultRepository)
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _memberService = memberService;
            _accountService = accountService;
            _memberManager = memberManager;
            _quizResultRepository = quizResultRepository;
        }

        public override IActionResult Index()
        {
            var email = "";
            IMember? member = null;
            var memberIdValue = ControllerContext?.HttpContext?.Request?.Query["memberId"].ToString() ?? "0";

            var isViewingOwnProfile = false;

            var loginPage = CurrentPage.AncestorOrSelf<HomePage>().FirstChildOfType(LoginPage.ModelTypeAlias);

            if (int.TryParse(memberIdValue, out var memberId))
            {
                //member id set
            }

            if (memberId > 0)
            {
                member = _memberService.GetById(memberId);
            }
            else if(User?.Identity?.IsAuthenticated ?? false)
            {
                email = User.Identity.GetEmail();
                member = _memberService.GetByEmail(email);
                isViewingOwnProfile = true;
            }

            if (member == null) return Redirect(loginPage?.Url() ?? "/");

            var memberModel = _accountService.GetMemberModelFromMember(member);
            var enrichedProfile = _accountService.GetEnrichedProfile(memberModel);

            if (enrichedProfile == null || (!isViewingOwnProfile && enrichedProfile.HideProfile)) return Redirect(loginPage?.Url() ?? "/");

            ProfileResultsViewModel resultsModel = new ProfileResultsViewModel();

            var playerRecord = _quizResultRepository.GetPlayerRecordByMemberId(member.Id);

            if (playerRecord != null)
            {
                playerRecord.Badges = enrichedProfile?.Badges?.Count() ?? 0;
                resultsModel.PlayerRecord = playerRecord;
            }

            resultsModel.Profile = enrichedProfile;

            var model = new ProfilePageContentModel(CurrentPage);
            model.ProfileResults = resultsModel;

            return CurrentTemplate(model);
        }
    }
}