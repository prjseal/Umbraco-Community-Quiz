using Quiz.Site.Models.Badges;
using Quiz.Site.Notifications.Member;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.NotificationHandlers.BadgeHandlers;

public class EarlyAdopterBadgeNotificationHandler : INotificationHandler<MemberRegisteredNotification>
{
    private readonly IBadgeService _badgeService;

    private readonly static DateTime EarlyAdopterThreshold = new(2022, 11, 5);

    public EarlyAdopterBadgeNotificationHandler(IBadgeService badgeService)
    {
        _badgeService = badgeService;
    }
    
    public void Handle(MemberRegisteredNotification notification)
    {
        if (DateTime.Now.Date < EarlyAdopterThreshold.Date)
        {
            _badgeService.AddBadgeToMember(notification.RegisteredMember, notification.Badges, new EarlyAdopterBadge());
        }
    }
}