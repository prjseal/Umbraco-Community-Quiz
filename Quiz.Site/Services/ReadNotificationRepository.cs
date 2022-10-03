using Quiz.Site.Enums;
using Quiz.Site.Models;
using Umbraco.Cms.Infrastructure.Scoping;

namespace Quiz.Site.Services;

public class ReadNotificationRepository : IReadNotificationRepository
{
    private static IScopeProvider _scopeProvider;

    public ReadNotificationRepository(IScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;
    }

    public IEnumerable<ReadNotification> GetAll()
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var records = db.Query<ReadNotification>("SELECT * FROM ReadNotification");

            return records;
        }
    }

    public ReadNotification GetById(int id)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var record = db.Query<ReadNotification>("SELECT * FROM ReadNotification WHERE [Id] = @Id", new { id }).FirstOrDefault();

            return record;
        }
    }

    public List<ReadNotification> GetByIds(int[] ids)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var joinedIds = string.Join(',', ids);
            var sql = $"SELECT * FROM ReadNotification WHERE [Id] IN ({joinedIds})";
            var db = scope.Database;
            var records = db.Query<ReadNotification>(sql).ToList();

            return records;
        }
    }

    public void Create(ReadNotification readNotification)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            readNotification.DateCreated = DateTime.UtcNow;
            scope.Database.Insert(readNotification);
            scope.Complete();
        }
    }

    public ReadNotification Update(ReadNotification readNotification)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            scope.Database.Update(readNotification);
            scope.Complete();
        }

        var item = GetById(readNotification.Id);

        return item;
    }

    public int Delete(int id)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var result = scope.Database.Delete<ReadNotification>("WHERE [Id] = @Id", new { id });
            scope.Complete();

            return result;
        }
    }
}
