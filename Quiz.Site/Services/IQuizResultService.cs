using Quiz.Site.Models;

namespace Quiz.Site.Services;
public interface IQuizResultService
{
    bool HasCompletedThisQuizBefore(int memberId, string quizPageUdi);
}
