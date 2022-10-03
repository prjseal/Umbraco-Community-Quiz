using Quiz.Site.Enums;
using Quiz.Site.Models;
using Umbraco.Cms.Infrastructure.Scoping;

namespace Quiz.Site.Services;

public class QuizResultRepository : IQuizResultRepository
{
    private static IScopeProvider _scopeProvider;

    public QuizResultRepository(IScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;
    }

    public IEnumerable<QuizResult> GetAll()
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var records = db.Query<QuizResult>("SELECT * FROM QuizResult");

            return records;
        }
    }

    public QuizResult GetById(int id)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var record = db.Query<QuizResult>("SELECT * FROM QuizResult WHERE [Id] = @Id", new { id }).FirstOrDefault();

            return record;
        }
    }

    public List<QuizResult> GetByIds(int[] ids)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var joinedIds = string.Join(',', ids);
            var sql = $"SELECT * FROM QuizResult WHERE [Id] IN ({joinedIds})";
            var db = scope.Database;
            var records = db.Query<QuizResult>(sql).ToList();

            return records;
        }
    }

    public QuizResult GetByMemberId(int memberId)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var record = db.Query<QuizResult>("SELECT * FROM QuizResult WHERE [MemberId] = @memberId", new { memberId }).FirstOrDefault();

            return record;
        }
    }

    public IEnumerable<QuizResult> GetAllByMemberId(int memberId)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var records = db.Query<QuizResult>("SELECT * FROM QuizResult WHERE [MemberId] = @memberId", new { memberId });

            return records;
        }
    }

    public void Create(QuizResult question)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            question.DateCreated = DateTime.UtcNow;
            scope.Database.Insert(question);
            scope.Complete();
        }
    }

    public QuizResult Update(QuizResult question)
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
            var result = scope.Database.Delete<QuizResult>("WHERE [Id] = @Id", new { id });
            scope.Complete();

            return result;
        }
    }

    public IEnumerable<PlayerRecord> GetPlayerRecords()
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var results = scope.Database.Fetch<PlayerRecord>("SELECT memberId as 'MemberId', SUM(score) as 'Correct', SUM(total) as Total, COUNT(score) as 'Quizzes' FROM QuizResult GROUP BY memberId");
            scope.Complete();

            return results;
        }
    }
}
