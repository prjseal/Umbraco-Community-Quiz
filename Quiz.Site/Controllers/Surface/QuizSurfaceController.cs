using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quiz.Site.Enums;
using Quiz.Site.Extensions;
using Quiz.Site.Models;
using Quiz.Site.Notifications.Quiz;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Mail;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
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
        private readonly IShortStringHelper _shortStringHelper;

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
            IEventAggregator eventAggregator,
            IShortStringHelper shortStringHelper
            ) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _memberManager = memberManager;
            _memberService = memberService;
            _logger = logger;
            _emailSender = emailSender;
            _globalSettings = globalSettings?.Value;
            _accountService = accountService;
            _questionService = questionService;
            _quizResultRepository = quizResultRepository;
            _badgeService = badgeService;
            _notificationRepository = notificationRepository;
            _memoryCache = memoryCache;
            _eventAggregator = eventAggregator;
            _shortStringHelper = shortStringHelper;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(QuizViewModel model)
        {
            if (!ModelState.IsValid)
            {
                    this._logger.LogInformation("Quiz form state invalid");
                await _eventAggregator.PublishAsync(new QuizCompletingFailedNotification("ModelState Invalid"));
                return CurrentUmbracoPage();
            }

                this._logger.LogInformation("Quiz form state is valid");


                this._logger.LogInformation("Getting quiz page by id " + model.QuizId);
            var quizContentPage = base.UmbracoContext.Content.GetById(model.QuizId);

            if (quizContentPage == null) throw new Exception("Quiz content page is null");

            var quizPage = (QuizPage)quizContentPage;
            var questionIds = quizPage?.Questions?.Select(x => int.Parse(x)).ToArray();

            if(questionIds == null || !questionIds.Any()) throw new Exception("No questions found for this quiz");

            var quizQuestionsCacheId = quizPage.Name.ToUrlSegment(_shortStringHelper);

                this._logger.LogInformation("Quiz questions cache id " + quizQuestionsCacheId);

            if (!_memoryCache.TryGetValue(quizQuestionsCacheId, out List<QuizQuestionViewModel> questionsToVerify))
            {
                this._logger.LogInformation("Quiz questions not in the cache for id " + quizQuestionsCacheId);
                questionsToVerify = _questionService.GetListOfQuestions(questionIds);

                _memoryCache.Set(quizQuestionsCacheId, questionsToVerify, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromHours(1),
                    Priority = CacheItemPriority.High,
                });
            }
            else
            {
                this._logger.LogInformation("Quiz questions returned from the cache. Cache id " + quizQuestionsCacheId);
            }

            int questionCount = model.Questions.Count;

                this._logger.LogInformation("Quiz question count " + questionCount);

            int correctCount = 0;
            var answers = VerifyQuestions(model, questionsToVerify, out correctCount);

                this._logger.LogInformation("Correct answers count " + correctCount);

            var member = await _memberManager.GetCurrentMemberAsync();

            if (member == null) throw new Exception("Unable to get current member");

                this._logger.LogInformation("Current member found " + member.Email);

            var memberItem = _memberService.GetByEmail(member.Email);

            if (memberItem == null) throw new Exception("Member item is null for email " + member.Email);

                this._logger.LogInformation("Current member item id " + memberItem.Id);

            var quizUdi = quizPage.GetUdiObject();

            if (quizUdi == null) throw new Exception("Quiz UDI is null");

            string answersModel = null;
            if(answers != null && answers.Any())
            {
                answersModel = JsonConvert.SerializeObject(answers);
            }

            var quizResult = new QuizResult()
            {
                Name = memberItem.Name + " - " + quizPage.Name,
                MemberId = memberItem.Id,
                QuizId = quizUdi.ToString(),
                Score = correctCount,
                Total = questionCount,
                Answers = answersModel
            };

            this._logger.LogInformation("Getting existing record");

            var existingRecord = _quizResultRepository.GetByMemberIdAndQuizId(memberItem.Id, quizUdi.ToString());

            if (existingRecord == null || existingRecord.Id <= 0)
            {
                this._logger.LogInformation("Creating new quiz record");
                _quizResultRepository.Create(quizResult);
            }
            else
            {
                this._logger.LogInformation("Existing record id " + existingRecord.Id);
            }

            this._logger.LogInformation("Sending quiz completed notification");

            await _eventAggregator.PublishAsync(new QuizCompletedNotification(memberItem, quizResult.Total, quizResult.Score, quizUdi.ToString()));

            this._logger.LogInformation("Back from quiz completed notification");

            if (_memoryCache.TryGetValue(CacheKey.LeaderBoard, out _))
            {
                this._logger.LogInformation("Clearing leaderboard cache");
                _memoryCache.Remove(CacheKey.LeaderBoard);
            }
            else
            {
                this._logger.LogInformation("Leaderboard was not cached");
            }

            TempData["QuizSubmitSuccess"] = true;

            this._logger.LogInformation("Redirecting to current page");

            return RedirectToCurrentUmbracoPage();
        }

        private List<AnswerModel> VerifyQuestions(QuizViewModel model, List<QuizQuestionViewModel> questionsToVerify, out int correctCount)
        {
            List<AnswerModel> answers = new List<AnswerModel>();
            int questionCount = model.Questions.Count;
            correctCount = 0;
            for (var q = 0; q < questionCount; q++)
            {
                var selectedAnswerIndex = model.Questions[q].Answer;
                var selectedAnswerIndexAsInt = int.Parse(selectedAnswerIndex);
                var correctAnswerIndex = questionsToVerify[q].CorrectAnswerPosition;
                var isCorrect = correctAnswerIndex == selectedAnswerIndexAsInt;

                answers.Add(new AnswerModel()
                {
                    CorrectAnswer = correctAnswerIndex.Value,
                    SubmittedAnswer = selectedAnswerIndexAsInt
                });

                questionsToVerify[q].IsCorrect = isCorrect;
                questionsToVerify[q].Answers[selectedAnswerIndexAsInt].Selected = true;
                if (isCorrect)
                {
                    correctCount++;
                }
            }
            return answers;
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
