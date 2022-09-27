using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Quiz.Site.Models;
using Quiz.Site.Models.ContentModels;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Controllers
{
    public class QuizPageController : RenderController
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IMemberManager _memberManager;
        private readonly IQuestionService _questionService;

        public QuizPageController(IQuestionRepository quesitonRepository, 
            ILogger<ProfilePageController> logger, 
            ICompositeViewEngine compositeViewEngine, 
            IUmbracoContextAccessor umbracoContextAccessor,
            IMemberManager memberManager,
            IQuestionService questionService)
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _questionRepository = quesitonRepository;
            _memberManager = memberManager;
            _questionService = questionService;
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
            var member = _memberManager.GetCurrentMemberAsync();
            

            var quiz = new QuizViewModel();
            quiz.QuizId = quizPage.Id;
            quiz.MemberId = member.Id;

            var questionIds = quizPage?.Questions?.Select(x => int.Parse(x)).ToArray();

            quiz.Questions = _questionService.GetListOfQuestions(questionIds);

            var model = new QuizPageContentModel(CurrentPage);
            model.Quiz = quiz;

            return CurrentTemplate(model);
        }
    }
}