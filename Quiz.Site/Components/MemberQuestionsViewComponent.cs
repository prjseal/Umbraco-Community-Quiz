using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;
using Quiz.Site.Services;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "MemberQuestions")]
    public class MemberQuestionsViewComponent : ViewComponent
    {
        private readonly IQuestionRepository _questionRepository;

        public MemberQuestionsViewComponent(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public IViewComponentResult Invoke()
        {
            IEnumerable<Question> questions = _questionRepository.GetAll();

            return View(questions);
        }
    }
}
