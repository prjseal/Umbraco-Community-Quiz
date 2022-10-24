using Quiz.Site.Models.Badges;
using Quiz.Site.Notifications.Member;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Events;

namespace Quiz.Site.NotificationHandlers.BadgeHandlers;

public class EarlyAdopterBadgeNotificationHandler : INotificationHandler<MemberRegisteredNotification>
{
    private readonly IBadgeService _badgeService;

    public EarlyAdopterBadgeNotificationHandler(IBadgeService badgeService)
    {
        _badgeService = badgeService;
    }
    
    public void Handle(MemberRegisteredNotification notification)
    {
        var badge = new EarlyAdopterBadge();
        _badgeService.AddBadgeToMember(notification.RegisteredMember, badge);
    }
}