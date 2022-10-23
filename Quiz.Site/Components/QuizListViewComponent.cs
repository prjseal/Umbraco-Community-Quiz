using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "QuizList")]
    public class QuizListViewComponent : ViewComponent
    {
        private readonly IQuizResultRepository _quizResultRepository;
        private readonly IAccountService _accountService;

        public QuizListViewComponent(IQuizResultRepository quizResultRepository, IAccountService accountService)
        {
            _quizResultRepository = quizResultRepository;
            _accountService = accountService;
        }

        public IViewComponentResult Invoke(MemberIdentityUser user, QuizListPage quizListPage)
        {
            var member = _accountService.GetMemberModelFromUser(user);

            IEnumerable<QuizPage> quizzes = Enumerable.Empty<QuizPage>();

            if(quizListPage != null && quizListPage.Children != null && quizListPage.Children.Any())
            {
                quizzes = quizListPage.Children.OrderByDescending(x => x.CreateDate).Select(x => (QuizPage)x);
            }

            var model = new QuizListViewModel
            {
                Quizzes = quizzes,
                Results = _quizResultRepository.GetAllByMemberId(member.Id)
            };

            return View(model);
        }
    }
}
