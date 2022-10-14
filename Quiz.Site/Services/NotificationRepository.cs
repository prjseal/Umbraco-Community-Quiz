using Quiz.Site.Models;
using Umbraco.Cms.Infrastructure.Scoping;

namespace Quiz.Site.Services;

public class NotificationRepository : INotificationRepository
{
    private static IScopeProvider _scopeProvider;

    public NotificationRepository(IScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;
    }

    public IEnumerable<Notification> GetAll()
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var records = db.Query<Notification>("SELECT * FROM Notification");

            return records;
        }
    }

    public IEnumerable<Notification> GetAllUnread()
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var records = db.Query<Notification>("SELECT[Notification].id, badgeId, [Notification].datecreated, memberId, message FROM Notification WHERE[Notification].id NOT IN(SELECT ReadNotification.id FROM ReadNotification)");

            return records;
        }
    }

    public Notification GetById(int id)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var record = db.Query<Notification>("SELECT * FROM Notification WHERE [Id] = @Id", new { id }).FirstOrDefault();

            return record;
        }
    }

    public List<Notification> GetByIds(int[] ids)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var joinedIds = string.Join(',', ids);
            var sql = $"SELECT * FROM Notification WHERE [Id] IN ({joinedIds})";
            var db = scope.Database;
            var records = db.Query<Notification>(sql).ToList();

            return records;
        }
    }

    public IEnumerable<Notification> GetAllByMemberId(int memberId)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var records = db.Query<Notification>("SELECT * FROM Notification WHERE [MemberId] = @memberId ORDER BY [DateCreated] DESC", new { memberId });

            return records;
        }
    }

    public IEnumerable<Notification> GetAllUnreadByMemberId(int memberId)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var db = scope.Database;
            var records = db.Query<Notification>("SELECT[Notification].id, badgeId, [Notification].datecreated, memberId, message FROM Notification WHERE[Notification].id NOT IN(SELECT ReadNotification.id FROM ReadNotification) AND [MemberId] = @memberId", new { memberId });

            return records;
        }
    }

    public void Create(Notification notification)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            notification.DateCreated = DateTime.UtcNow;
            scope.Database.Insert(notification);
            scope.Complete();
        }
    }

    public Notification Update(Notification notification)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            scope.Database.Update(notification);
            scope.Complete();
        }

        var item = GetById(notification.Id);

        return item;
    }

    public int Delete(int id)
    {
        using (var scope = _scopeProvider.CreateScope())
        {
            var result = scope.Database.Delete<Notification>("WHERE [Id] = @Id", new { Id = id });
            scope.Complete();

            return result;
        }
    }
}
