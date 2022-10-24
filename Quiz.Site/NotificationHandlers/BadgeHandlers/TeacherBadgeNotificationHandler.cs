using Quiz.Site.Models.Badges;
using Quiz.Site.Notifications.Question;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Events;

namespace Quiz.Site.NotificationHandlers.BadgeHandlers;

public class TeacherBadgeNotificationHandler : INotificationHandler<QuestionCreatedNotification>
{
    private readonly IBadgeService _badgeService;

    public TeacherBadgeNotificationHandler(IBadgeService badgeService)
    {
        _badgeService = badgeService;
    }
    
    public void Handle(QuestionCreatedNotification notification)
    {
        _badgeService.AddBadgeToMember(notification.CreatedBy, new TeacherBadge());
    }
}