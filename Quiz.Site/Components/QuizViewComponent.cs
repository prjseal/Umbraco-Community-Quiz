using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "Quiz")]
    public class QuizViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(QuizViewModel model)
        {
            return View(model);
        }
    }
}
