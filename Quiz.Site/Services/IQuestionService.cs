using Quiz.Site.Models;

namespace Quiz.Site.Services;
public interface IQuestionService
{
    List<QuizQuestionViewModel> GetListOfQuestions(int[] questionIds);
}
