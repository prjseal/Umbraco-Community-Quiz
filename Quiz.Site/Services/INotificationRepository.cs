using Quiz.Site.Enums;
using Quiz.Site.Models;

namespace Quiz.Site.Services;

public interface INotificationRepository
{
    IEnumerable<Notification> GetAll();

    IEnumerable<Notification> GetAllUnread();

    Notification GetById(int id);

    List<Notification> GetByIds(int[] ids);

    IEnumerable<Notification> GetAllByMemberId(int memberId);

    IEnumerable<Notification> GetAllUnreadByMemberId(int memberId);

    void Create(Notification Notification);

    Notification Update(Notification Notification);

    int Delete(int id);
}