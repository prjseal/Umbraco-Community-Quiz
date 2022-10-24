using Quiz.Site.Models.Badges;
using Quiz.Site.Notifications.Profile;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Events;

namespace Quiz.Site.NotificationHandlers.BadgeHandlers;

public class UpdatedProfileBadgeNotificationHandler : INotificationHandler<ProfileUpdatedNotification>
{
    private readonly IBadgeService _badgeService;

    public UpdatedProfileBadgeNotificationHandler(IBadgeService badgeService)
    {
        _badgeService = badgeService;
    }

    public void Handle(ProfileUpdatedNotification notification)
    {
        _badgeService.AddBadgeToMember(notification.UpdatedBy, new UpdatedProfileBadge());
    }
}