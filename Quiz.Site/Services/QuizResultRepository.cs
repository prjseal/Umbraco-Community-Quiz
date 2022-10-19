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
            scope.Complete();

            return records;
        }
    }

    public QuizResult GetById(int id)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var record = db.Query<QuizResult>("SELECT * FROM QuizResult WHERE [Id] = @id", new { id }).FirstOrDefault();
            scope.Complete();

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
            scope.Complete();

            return records;
        }
    }

    public QuizResult GetByMemberId(int memberId)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var record = db.Query<QuizResult>("SELECT * FROM QuizResult WHERE [MemberId] = @memberId", new { memberId }).FirstOrDefault();
            scope.Complete();

            return record;
        }
    }

    public IEnumerable<QuizResult> GetAllByMemberId(int memberId)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var records = db.Query<QuizResult>("SELECT * FROM QuizResult WHERE [MemberId] = @memberId", new { memberId });
            scope.Complete();

            return records;
        }
    }

    public QuizResult GetByMemberIdAndQuizId(int memberId, string quizId)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var records = db.Query<QuizResult>("SELECT * FROM QuizResult WHERE [MemberId] = @memberId AND [quizId] = @quizId", new { memberId, quizId }).FirstOrDefault();
            scope.Complete();

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

    public void Delete(int Id)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            scope.Database.Delete<QuizResult>("WHERE [Id] = @Id", new { Id });
            scope.Complete();
        }
    }

    public IEnumerable<PlayerRecord> GetPlayerRecords()
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var results = scope.Database.Fetch<PlayerRecord>("SELECT memberId as 'MemberId', SUM(score) as 'Correct', SUM(total) as Total, COUNT(score) as 'Quizzes', MAX(datecreated) as 'DateOfLastQuiz' FROM QuizResult GROUP BY memberId");
            scope.Complete();
            return results;
        }
    }

    public PlayerRecord GetPlayerRecordByMemberId(int memberId)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var result = scope.Database.Fetch<PlayerRecord>("SELECT memberId as 'MemberId', SUM(score) as 'Correct', SUM(total) as Total, COUNT(score) as 'Quizzes', MAX(datecreated) as 'DateOfLastQuiz' FROM QuizResult WHERE memberId = @memberId", new { memberId }).FirstOrDefault();
            scope.Complete();
            return result;
        }
    }
}
