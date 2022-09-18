using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Models;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "SubmitQuestion")]
    public class SubmitQuestionComponent : ViewComponent
    {
        public IViewComponentResult Invoke(QuestionViewModel model)
        {
            return View(model);
        }
    }
}
