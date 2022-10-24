using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;
using Quiz.Site.Extensions;
using Quiz.Site.Models;
using Quiz.Site.Models.ContentModels;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
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

        public QuizPageController(IQuestionRepository questionRepository,
            ILogger<ProfilePageController> logger,
            ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor,
            IMemberManager memberManager,
            IMemberService memberService,
            IQuestionService questionService,
            IQuizResultService quizResultService)
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _questionRepository = questionRepository;
            _memberManager = memberManager;
            _memberService = memberService;
            _questionService = questionService;
            _quizResultService = quizResultService;
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

            var completedPreviously = _quizResultService.HasCompletedThisQuizBefore(memberItem.Id, quizPage.GetUdiObject().ToString());
            var completedQuiz = TempData["CompletedQuiz"] != null ? JsonConvert.DeserializeObject<QuizViewModel>(TempData["CompletedQuiz"].ToString()) : null;

            var quiz = new QuizViewModel();

            if (completedQuiz is null)
            {
                quiz.CompletedPreviously = completedPreviously;
                quiz.QuizId = quizPage.Id;
                quiz.MemberId = memberItem.Id;

                var questionIds = quizPage?.Questions?.Select(x => int.Parse(x)).ToArray();

                quiz.Questions = _questionService.GetListOfQuestions(questionIds);

                if ((completedQuiz is null || completedQuiz.QuizId != quizPage.Id) && completedPreviously)
                {
                    SetAllCorrectAnswers(quiz.Questions);
                }
            }
            else
            {
                quiz = completedQuiz;
            }

            var model = new QuizPageContentModel(CurrentPage);
            model.Quiz = quiz;

            return CurrentTemplate(model);
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