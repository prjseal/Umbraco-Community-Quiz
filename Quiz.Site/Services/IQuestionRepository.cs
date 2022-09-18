using Quiz.Site.Models;

namespace Quiz.Site.Services;

public interface IQuestionRepository
{
    IEnumerable<Question> GetAll();

    Question GetById(int id);

    void Create(Question question);

    Question Update(Question question);

    int Delete(int id);
}