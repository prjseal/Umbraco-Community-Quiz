using Quiz.Site.Models;

namespace Quiz.Site.Services;

public interface IQuestionRepository
{
    IEnumerable<Question> GetAll();

    Question GetById(int id);

    Question Create(Question question);

    Question Update(Question question);

    bool Delete(Question question);
}