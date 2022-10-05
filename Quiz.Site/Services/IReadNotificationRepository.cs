using Quiz.Site.Models;

namespace Quiz.Site.Services;

public interface IReadNotificationRepository
{
    IEnumerable<ReadNotification> GetAll();

    ReadNotification GetById(int id);

    List<ReadNotification> GetByIds(int[] ids);

    void Create(ReadNotification ReadNotification);

    ReadNotification Update(ReadNotification readNotification);

    int Delete(int id);
}