using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Quiz.Site.Enums;
using Quiz.Site.Extensions;
using Quiz.Site.Models;
using Quiz.Site.Services;
using System.Security.Cryptography;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Mail;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.PublishedModels;
using Umbraco.Cms.Web.Website.Controllers;

namespace Quiz.Site.Controllers.Surface
{
    public class QuestionSurfaceController : SurfaceController
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly ILogger<RegisterSurfaceController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly GlobalSettings _globalSettings;
        private readonly IMemberManager _memberManager;
        private readonly IMemberService _memberService;
        private readonly IAccountService _accountService;
        private readonly IBadgeService _badgeService;
        private readonly INotificationRepository _notificationRepository;

        public QuestionSurfaceController(
            //these are required by the base controller
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            //these are dependencies we've added
            IQuestionRepository questionRepository,
            ILogger<RegisterSurfaceController> logger,
            IEmailSender emailSender,
            IOptions<GlobalSettings> globalSettings,
            IMemberManager memberManager,
            IMemberService memberService,
            IAccountService accountService,
            IBadgeService badgeService,
            INotificationRepository notificationRepository
            ) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _globalSettings = globalSettings?.Value ?? throw new ArgumentNullException(nameof(globalSettings));
            _memberManager = memberManager;
            _memberService = memberService;
            _accountService = accountService;
            _badgeService = badgeService ?? throw new ArgumentNullException(nameof(badgeService));
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(QuestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            GuidUdi udi = null;
            var userEmail = User?.Identity?.GetEmail();

            IMember member = null;
            Umbraco.Cms.Web.Common.PublishedModels.Member memberModel = null;
            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                member = _memberService.GetByEmail(userEmail);
                if (member != null)
                {
                    udi = member.GetUdi();
                    memberModel = _accountService.GetMemberModelFromMember(member);
                }
            }

            if (udi == null)
            {
                ModelState.AddModelError("", "Not logged in");
                return CurrentUmbracoPage();
            }

            var randomPosition = GetRandomNumberInRange(0, 4);
            Question question = new Question()
            {
                Id = model.Id,
                AuthorMemberId = udi.ToString(),
                QuestionText = model.QuestionText,
                CorrectAnswer = model.CorrectAnswer,
                WrongAnswer1 = model.WrongAnswer1,
                WrongAnswer2 = model.WrongAnswer2,
                WrongAnswer3 = model.WrongAnswer3,
                MoreInfoLink = model.MoreInfoLink,
                Status = ((int)QuestionStatus.Pending).ToString(),
                CorrectAnswerPosition = randomPosition
            };

            _questionRepository.Create(question);

            var teacherBadge = _badgeService.GetBadgeByName("Teacher");
            if (!_badgeService.HasBadge(memberModel, teacherBadge))
            {
                if (_badgeService.AddBadgeToMember(member, teacherBadge))
                {
                    _notificationRepository.Create(new Models.Notification()
                    {
                        BadgeId = teacherBadge.GetUdiObject().ToString(),
                        MemberId = memberModel.Id,
                        Message = "New badge earned - " + teacherBadge.Name
                    });

                    TempData["ShowToast"] = true;
                }
            }

            var profilePage = CurrentPage.AncestorOrSelf<HomePage>().FirstChildOfType(ProfilePage.ModelTypeAlias);

            return RedirectToUmbracoPage(profilePage);
        }

        private static int GetRandomNumberInRange(int min, int max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException();

            var _rng = RandomNumberGenerator.Create();

            var data = new byte[sizeof(int)];
            _rng.GetBytes(data);
            var randomNumber = BitConverter.ToInt32(data, 0);

            return (int)Math.Floor((double)(min + Math.Abs(randomNumber % (max - min))));
        }
    }
}
