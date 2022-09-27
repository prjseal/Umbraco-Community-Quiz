using Quiz.Site.Enums;
using Quiz.Site.Models;

namespace Quiz.Site.Services;

public interface IQuestionRepository
{
    IEnumerable<Question> GetAll();

    IEnumerable<Question> GetAllByStatus(QuestionStatus questionStatus);

    Question GetById(int id);

    List<Question> GetByIds(int[] ids);

    Question GetByMemberId(string memberId);

    void Create(Question question);

    Question Update(Question question);

    int Delete(int id);
}