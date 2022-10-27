using Quiz.Site.Models.Badges;
using Quiz.Site.Notifications.Quiz;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Events;

namespace Quiz.Site.NotificationHandlers.BadgeHandlers;

public class PerfectScoreBadgeNotificationHandler : INotificationHandler<QuizCompletedNotification>
{
    private readonly IBadgeService _badgeService;

    public PerfectScoreBadgeNotificationHandler(IBadgeService badgeService)
    {
        _badgeService = badgeService;
    }

    public void Handle(QuizCompletedNotification notification)
    {
        if (notification.QuizScore >= notification.QuizTotal)
        {
            _badgeService.AddBadgeToMember(notification.CompletedBy, notification.Badges, new PerfectScoreBadge());
        }
    }
}