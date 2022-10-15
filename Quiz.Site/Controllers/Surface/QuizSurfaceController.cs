using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quiz.Site.Enums;
using Quiz.Site.Extensions;
using Quiz.Site.Models;
using Quiz.Site.Notifications;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Events;
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
        private readonly IBadgeService _badgeService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IEventAggregator _eventAggregator;

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
            IQuizResultRepository quizResultRepository,
            IBadgeService badgeService,
            INotificationRepository notificationRepository,
            IMemoryCache memoryCache,
            IEventAggregator eventAggregator
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
            _badgeService = badgeService ?? throw new ArgumentNullException(nameof(badgeService));
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _memoryCache = memoryCache ?? throw new ArgumentException(nameof(memoryCache));
            _eventAggregator = eventAggregator ?? throw new ArgumentException(nameof(eventAggregator));
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

            int questionCount = model.Questions.Count;
            int correctCount = 0;
            VerifyQuestions(model, questionsToVerify, out correctCount);

            var member = await _memberManager.GetCurrentMemberAsync();
            var memberItem = _memberService.GetByEmail(member.Email);
            var memberModel = _accountService.GetMemberModelFromMember(memberItem);

            var quiz = new QuizViewModel();
            quiz.QuizId = quizPage.Id;
            quiz.MemberId = memberItem.Id;
            quiz.Questions = questionsToVerify;

            var quizUdi = quizPage.GetUdiObject();

            var quizResult = new QuizResult()
            {
                Name = memberItem.Name + " - " + quizPage.Name, 
                MemberId = memberItem.Id,
                QuizId = quizUdi.ToString(),
                Score = correctCount,
                Total = questionCount
            };

            _quizResultRepository.Create(quizResult);
            
            await _eventAggregator.PublishAsync(new QuizCompletedNotification(memberItem, quizResult.Total, quizResult.Score));

            if(quizResult.Score > 0 && quizResult.Score == quizResult.Total)
            {
                var badge = _badgeService.GetBadgeByName("Perfect Score");
                if (!_badgeService.HasBadge(memberModel, badge))
                {
                    if (_badgeService.AddBadgeToMember(memberItem, badge))
                    {
                        _notificationRepository.Create(new Notification()
                        {
                            BadgeId = badge.GetUdiObject().ToString(),
                            MemberId = memberModel.Id,
                            Message = "New badge earned - " + badge.Name
                        });

                        TempData["ShowToast"] = true;
                    }
                }
            }

            if (_memoryCache.TryGetValue(CacheKey.LeaderBoard, out _))
            {
                _memoryCache.Remove(CacheKey.LeaderBoard);
            }

            TempData["Success"] = true;
            TempData["CompletedQuiz"] = JsonConvert.SerializeObject(quiz);

            return RedirectToCurrentUmbracoPage();
        }

        private static void VerifyQuestions(QuizViewModel model, List<QuizQuestionViewModel> questionsToVerify, out int correctCount)
        {
            int questionCount = model.Questions.Count;
            correctCount = 0;
            for (var q = 0; q < questionCount; q++)
            {
                var selectedAnswerIndex = model.Questions[q].Answer;
                var selectedAnswerIndexAsInt = int.Parse(selectedAnswerIndex);
                var isCorrect = questionsToVerify[q].CorrectAnswerPosition == selectedAnswerIndexAsInt;
                questionsToVerify[q].IsCorrect = isCorrect;
                questionsToVerify[q].Answers[selectedAnswerIndexAsInt].Selected = true;
                if (isCorrect)
                {
                    correctCount++;
                }
            }
        }

        private static void SetAllCorrectAnswers(List<QuizQuestionViewModel> questionsToVerify)
        {
            int questionCount = questionsToVerify?.Count ?? 0;
            for (var q = 0; q < questionCount; q++)
            {
                var selectedAnswerIndexAsInt = questionsToVerify[q].CorrectAnswerPosition.Value;
                questionsToVerify[q].IsCorrect = true;
                questionsToVerify[q].Answers[selectedAnswerIndexAsInt].Selected = true;
            }
        }
    }
}
