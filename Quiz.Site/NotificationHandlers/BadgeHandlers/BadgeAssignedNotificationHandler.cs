using Quiz.Site.Extensions;
using Quiz.Site.Models;
using Quiz.Site.Notifications.Badge;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Events;

namespace Quiz.Site.NotificationHandlers.BadgeHandlers;

public class BadgeAssignedNotificationHandler : INotificationHandler<BadgeAssignedNotification>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IBadgeService _badgeService;

    public BadgeAssignedNotificationHandler(INotificationRepository notificationRepository, IBadgeService badgeService)
    {
        _notificationRepository = notificationRepository;
        _badgeService = badgeService;
    }

    public void Handle(BadgeAssignedNotification notification)
    {
        var badge = _badgeService.GetBadgeByName(notification.Badge.UniqueUmbracoName);
        if (badge != null)
        {
            _notificationRepository.Create(new Notification()
            {
                BadgeId = badge.GetUdiObject().ToString(),
                MemberId = notification.AssignedTo.Id,
                Message = "New badge earned - " + notification.Badge.UniqueUmbracoName
            });
        }
    }
}