using Quiz.Site.Models;

namespace Quiz.Site.Services;

public interface IQuizResultRepository
{
    IEnumerable<QuizResult> GetAll();

    QuizResult GetById(int id);

    List<QuizResult> GetByIds(int[] ids);

    QuizResult GetByMemberId(int memberId);

    IEnumerable<QuizResult> GetAllByMemberId(int memberId);

    QuizResult GetByMemberIdAndQuizId(int memberId, string quizId);

    void Create(QuizResult quizResult);

    QuizResult Update(QuizResult quizResult);

    void Delete(int id);

    IEnumerable<PlayerRecord> GetPlayerRecords();
}