using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;
using Quiz.Site.Services;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "Quiz")]
    public class QuizViewComponent : ViewComponent
    {
        private readonly IQuizResultRepository _quizResultRepository;

        public QuizViewComponent(IQuizResultRepository quizResultRepository)
        {
            _quizResultRepository = quizResultRepository;
        }

        public IViewComponentResult Invoke(QuizViewModel model)
        {
            return View(model);
        }
    }
}
