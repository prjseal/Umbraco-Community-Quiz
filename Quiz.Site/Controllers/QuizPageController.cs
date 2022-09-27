using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
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

        public QuizPageController(IQuestionRepository quesitonRepository, 
            ILogger<ProfilePageController> logger, 
            ICompositeViewEngine compositeViewEngine, 
            IUmbracoContextAccessor umbracoContextAccessor,
            IMemberManager memberManager)
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _questionRepository = quesitonRepository;
            _memberManager = memberManager;
        }

        public override IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                var loginPage = CurrentPage.AncestorOrSelf<HomePage>().FirstChildOfType(LoginPage.ModelTypeAlias);

                return Redirect(loginPage?.Url() ?? "/");
            }

            var quizId = 1;
            var questionId = 1;

            var member = _memberManager.GetCurrentMemberAsync();
            

            var quiz = new QuizViewModel();
            quiz.QuizId = quizId;
            quiz.MemberId = member.Id;

            var questionIds = new[] { 1, 2 };

            quiz.Questions = GetListOfQuestions(questionIds);

            var model = new QuizPageContentModel(CurrentPage);
            model.Quiz = quiz;

            return CurrentTemplate(model);
        }

        private List<QuizQuestionViewModel> GetListOfQuestions(int[] questionIds)
        {
            List<QuizQuestionViewModel> quizQuestions;
            var questions = _questionRepository.GetByIds(questionIds);
            
            quizQuestions = new List<QuizQuestionViewModel>();
            if (questions != null && questions.Any())
            {
                foreach (var question in questions)
                {
                    List<SelectListItem> answers = new List<SelectListItem>();
                    var wrongCount = 0;
                    for (var i = 0; i < 4; i++)
                    {
                        if (question.CorrectAnswerPosition == i)
                        {
                            answers.Add(new SelectListItem() { Value = i.ToString(), Text = question.CorrectAnswer });
                        }
                        else
                        {
                            wrongCount++;
                            switch (wrongCount)
                            {
                                case 1:
                                    answers.Add(new SelectListItem() { Value = i.ToString(), Text = question.WrongAnswer1 });
                                    break;
                                case 2:
                                    answers.Add(new SelectListItem() { Value = i.ToString(), Text = question.WrongAnswer2 });
                                    break;
                                case 3:
                                    answers.Add(new SelectListItem() { Value = i.ToString(), Text = question.WrongAnswer3 });
                                    break;
                            }
                        }
                    }

                    var quizQuestion = new QuizQuestionViewModel();

                    quizQuestion.QuestionId = question.Id;
                    quizQuestion.QuestionText = question.QuestionText;
                    quizQuestion.CorrectAnswerPosition = question.CorrectAnswerPosition;
                    quizQuestion.Answers = answers;

                    quizQuestions.Add(quizQuestion);
                }
            }

            return quizQuestions;
        }
    }
}