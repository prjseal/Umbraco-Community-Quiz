using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
    public class QuizSurfaceController : SurfaceController
    {
        private readonly IMemberManager _memberManager;
        private readonly IMemberService _memberService;
        private readonly ILogger<ProfileSurfaceController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly GlobalSettings _globalSettings;
        private readonly IAccountService _accountService;
        private readonly IQuestionService _questionService;
        private readonly IQuizResultRepository _quizResultRepository;

        public QuizSurfaceController(
            //these are required by the base controller
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            //these are dependencies we've added
            IMemberManager memberManager,
            IMemberService memberService,
            ILogger<ProfileSurfaceController> logger,
            IEmailSender emailSender,
            IOptions<GlobalSettings> globalSettings,
            IAccountService accountService,
            IQuestionService questionService,
            IQuizResultRepository quizResultRepository
            ) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _memberManager = memberManager ?? throw new ArgumentNullException(nameof(memberManager));
            _memberService = memberService ?? throw new ArgumentNullException(nameof(memberService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _globalSettings = globalSettings?.Value ?? throw new ArgumentNullException(nameof(globalSettings));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _questionService = questionService ?? throw new ArgumentNullException(nameof(questionService));
            _quizResultRepository = quizResultRepository ?? throw new ArgumentNullException(nameof(quizResultRepository));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(QuizViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            var quizContentPage = base.UmbracoContext.Content.GetById(model.QuizId);
            var quizPage = (QuizPage)quizContentPage;
            var questionIds = quizPage?.Questions?.Select(x => int.Parse(x)).ToArray();

            var questionsToVerify = _questionService.GetListOfQuestions(questionIds);

            var questionCount = model.Questions.Count;
            var correctCount = 0;
            for(var q = 0; q < questionCount; q++)
            {
                var selectedAnswerIndex = model.Questions[q].Answer;
                var selectedAnswerIndexAsInt = int.Parse(selectedAnswerIndex);
                var isCorrect = questionsToVerify[q].CorrectAnswerPosition == selectedAnswerIndexAsInt;
                questionsToVerify[q].IsCorrect = isCorrect;
                questionsToVerify[q].Answers[selectedAnswerIndexAsInt].Selected = true;
                if(isCorrect)
                {
                    correctCount++;
                }
            }

            var member = _memberManager.GetCurrentMemberAsync();

            var quiz = new QuizViewModel();
            quiz.QuizId = quizPage.Id;
            quiz.MemberId = member.Id;
            quiz.Questions = questionsToVerify;

            GuidUdi udi = null;
            var userEmail = User?.Identity?.GetEmail();

            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                var memberAccount = _memberService.GetByEmail(userEmail);
                if (memberAccount != null)
                {
                    udi = memberAccount.GetUdi();
                }
            }

            var quizUdi = Udi.Create("document", quizPage.Key);

            var quizResult = new QuizResult()
            {
                MemberId = udi.ToString(),
                QuizId = quizUdi.ToString(),
                Score = correctCount,
                Total = questionCount
            };

            _quizResultRepository.Create(quizResult);

            TempData["Success"] = true;
            TempData["CompletedQuiz"] = JsonConvert.SerializeObject(quiz);

            return RedirectToCurrentUmbracoPage();
        }
    }
}
