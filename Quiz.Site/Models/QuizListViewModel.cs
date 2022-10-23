using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Models;

public class QuizListViewModel
{
    public IEnumerable<QuizResult> Results { get; set; }
    public IEnumerable<QuizPage> Quizzes { get; set; }
}
