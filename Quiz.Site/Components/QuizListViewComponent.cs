using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "QuizList")]
    public class QuizListViewComponent : ViewComponent
    {
        private readonly IQuizResultRepository _quizResultRepository;
        private readonly IAccountService _accountService;
        private readonly IMemberService _memberService;

        public QuizListViewComponent(IQuizResultRepository quizResultRepository, IAccountService accountService, IMemberService memberService)
        {
            _quizResultRepository = quizResultRepository;
            _accountService = accountService;
            _memberService = memberService;
        }

        public IViewComponentResult Invoke(MemberIdentityUser user, QuizListPage quizListPage)
        {
            var member = _memberService.GetByEmail(user.Email);
            var memberModel = _accountService.GetMemberModelFromMember(member);
            var enrichedProfile = _accountService.GetEnrichedProfile(memberModel);

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
