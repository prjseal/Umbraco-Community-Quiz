using Quiz.Site.Models.Badges;
using Quiz.Site.Notifications.Question;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.NotificationHandlers.BadgeHandlers;

public class TeacherBadgeNotificationHandler : INotificationHandler<QuestionCreatedNotification>
{
    private readonly IBadgeService _badgeService;
    private readonly IAccountService _accountService;

    public TeacherBadgeNotificationHandler(IBadgeService badgeService, IAccountService accountService)
    {
        _badgeService = badgeService;
        _accountService = accountService;
    }
    
    public void Handle(QuestionCreatedNotification notification)
    {
        var memberModel = _accountService.GetMemberModelFromMember(notification.CreatedBy);
        var enrichedProfile = _accountService.GetEnrichedProfile(memberModel);
        var badges = enrichedProfile?.Badges ?? Enumerable.Empty<BadgePage>();

        _badgeService.AddBadgeToMember(notification.CreatedBy, badges, new TeacherBadge());
    }
}