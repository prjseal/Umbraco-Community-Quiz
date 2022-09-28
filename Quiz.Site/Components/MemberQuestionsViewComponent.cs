using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;
using Quiz.Site.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "MemberQuestions")]
    public class MemberQuestionsViewComponent : ViewComponent
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IMemberService _memberService;

        public MemberQuestionsViewComponent(IQuestionRepository questionRepository, IMemberService memberService)
        {
            _questionRepository = questionRepository;
            _memberService = memberService;
        }

        public IViewComponentResult Invoke(MemberIdentityUser memberUser)
        {

            GuidUdi udi = null;
            if (!string.IsNullOrWhiteSpace(memberUser.Email))
            {
                var member = _memberService.GetByEmail(memberUser.Email);
                if (member != null)
                {
                    udi = member.GetUdi();
                }
            }

            IEnumerable<Question> questions = _questionRepository.GetByMemberId(udi.ToString());

            return View(questions);
        }
    }
}
