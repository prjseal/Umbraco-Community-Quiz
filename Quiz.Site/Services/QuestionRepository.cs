using Quiz.Site.Models;
using Umbraco.Cms.Infrastructure.Scoping;

namespace Quiz.Site.Services;

public class QuestionRepository : IQuestionRepository
{
    private static IScopeProvider _scopeProvider;

    public QuestionRepository(IScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;
    }

    public IEnumerable<Question> GetAll()
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var records = db.Query<Question>("SELECT * FROM Question");

            return records;
        }
    }

    public Question GetById(int id)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var record = db.Query<Question>("SELECT * FROM Question WHERE [Id] = @Id", new { id }).FirstOrDefault();

            return record;
        }
    }
    public void Create(Question question)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            scope.Database.Insert(question);
            scope.Complete();
        }
    }

    public Question Update(Question question)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            scope.Database.Update(question);
            scope.Complete();
        }

        var item = GetById(question.Id);

        return item;
    }

    public int Delete(int id)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var result = scope.Database.Delete<Question>("WHERE [Id] = @Id", new { id });
            scope.Complete();

            return result;
        }
    }
}
