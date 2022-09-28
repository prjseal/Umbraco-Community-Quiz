using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Quiz.Site.Enums;
using Quiz.Site.Models;
using Quiz.Site.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Mail;
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
            IMemberService memberService
            ) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _globalSettings = globalSettings?.Value ?? throw new ArgumentNullException(nameof(globalSettings));
            _memberManager = memberManager;
            _memberService = memberService;
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

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                var member = _memberService.GetByEmail(userEmail);
                if (member != null)
                {
                    udi = member.GetUdi();
                }
            }

            if (udi == null)
            {
                ModelState.AddModelError("", "Not logged in");
                return CurrentUmbracoPage();
            }

            var random = new Random();
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
                CorrectAnswerPosition = random.Next(0, 3)
            };

            _questionRepository.Create(question);

            var profilePage = CurrentPage.AncestorOrSelf<HomePage>().FirstChildOfType(ProfilePage.ModelTypeAlias);

            return RedirectToUmbracoPage(profilePage);
        }
    }
}
