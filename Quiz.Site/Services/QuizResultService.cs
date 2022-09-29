namespace Quiz.Site.Services;
public class QuizResultService : IQuizResultService
{
    private readonly IQuizResultRepository _quizResultRepository;

    public QuizResultService(IQuizResultRepository quizResultRepository)
    {
        _quizResultRepository = quizResultRepository;
    }

    public bool HasCompletedThisQuizBefore(int memberId, string quizPageUdi)
    {
        var quizzes = _quizResultRepository.GetAllByMemberId(memberId);
        if(quizzes != null && quizzes.Any())
        {
            return quizzes.Any(x => x.QuizId == quizPageUdi);
        }
        return false;
    }
}
