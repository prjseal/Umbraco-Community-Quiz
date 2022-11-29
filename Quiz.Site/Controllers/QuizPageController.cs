using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Quiz.Site.Extensions;
using Quiz.Site.Models;
using Quiz.Site.Models.ContentModels;
using Quiz.Site.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Controllers
{
    public class QuizPageController : RenderController
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IMemberManager _memberManager;
        private readonly IMemberService _memberService;
        private readonly IQuestionService _questionService;
        private readonly IQuizResultService _quizResultService;
        private readonly IMemoryCache _memoryCache;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IQuizResultRepository _quizResultRepository;

        public QuizPageController(IQuestionRepository questionRepository,
            ILogger<ProfilePageController> logger,
            ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor,
            IMemberManager memberManager,
            IMemberService memberService,
            IQuestionService questionService,
            IMemoryCache memoryCache,
            IQuizResultService quizResultService,
            IShortStringHelper shortStringHelper,
            IQuizResultRepository quizResultRepository)
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _questionRepository = questionRepository;
            _memberManager = memberManager;
            _memberService = memberService;
            _questionService = questionService;
            _quizResultService = quizResultService;
            _memoryCache = memoryCache;
            _shortStringHelper = shortStringHelper;
            _quizResultRepository = quizResultRepository;
        }

        public override IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                var loginPage = CurrentPage.AncestorOrSelf<HomePage>().FirstChildOfType(LoginPage.ModelTypeAlias);

                return Redirect(loginPage?.Url() ?? "/");
            }

            var quizPage = (QuizPage)CurrentPage;

            var quizId = quizPage.Id;
            var userEmail = User.Identity.GetEmail();
            var memberItem = _memberService.GetByEmail(userEmail);
            var quizUdi = quizPage.GetUdiObject();

            var completedQuiz = _quizResultRepository.GetByMemberIdAndQuizId(memberItem.Id, quizUdi.ToString());
            var completedPreviously = completedQuiz != null;

            var quiz = new QuizViewModel();

            quiz.CompletedPreviously = completedPreviously;
            quiz.QuizId = quizPage.Id;
            quiz.MemberId = memberItem.Id;

            var questionIds = quizPage?.Questions?.Select(x => int.Parse(x)).ToArray();
            var quizQuestionsCacheId = quizPage.Name.ToUrlSegment(_shortStringHelper);

            quiz.Questions = _questionService.GetListOfQuestions(questionIds);

            if (completedPreviously)
            {
                List<AnswerModel> answersModel = null;
                if(!string.IsNullOrWhiteSpace(completedQuiz.Answers))
                {
                    answersModel = JsonConvert.DeserializeObject<List<AnswerModel>>(completedQuiz.Answers);
                }
                SetAllCorrectAnswers(quiz.Questions, answersModel);
            }

            var model = new QuizPageContentModel(CurrentPage);
            model.Quiz = quiz;

            return CurrentTemplate(model);
        }

        private static void SetAllCorrectAnswers(List<QuizQuestionViewModel> questionsToVerify, 
            List<AnswerModel> answers)
        {
            int questionCount = questionsToVerify?.Count ?? 0;

            if(answers != null && answers.Any())
            {
                for (var q = 0; q < questionCount; q++)
                {
                    var answerModel = answers[q];
                    questionsToVerify[q].IsCorrect = answerModel.IsCorrect;
                    questionsToVerify[q].Answers[answerModel.SubmittedAnswer].Selected = true;
                }
            }
            else
            {
                for (var q = 0; q < questionCount; q++)
                {
                    var selectedAnswerIndexAsInt = questionsToVerify[q].CorrectAnswerPosition.Value;
                    questionsToVerify[q].IsCorrect = true;
                    questionsToVerify[q].Answers[selectedAnswerIndexAsInt].Selected = true;
                }
            }
        }
    }
}