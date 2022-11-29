using Quiz.Site.Models.Badges;
using Quiz.Site.Notifications.Quiz;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.NotificationHandlers.BadgeHandlers;

public class PerfectScoreBadgeNotificationHandler : INotificationHandler<QuizCompletedNotification>
{
    private readonly IBadgeService _badgeService;
    private readonly IAccountService _accountService;

    public PerfectScoreBadgeNotificationHandler(IBadgeService badgeService, IAccountService accountService)
    {
        _badgeService = badgeService;
        _accountService = accountService;
    }

    public void Handle(QuizCompletedNotification notification)
    {
        if (notification.QuizScore >= notification.QuizTotal)
        {
            var memberModel = _accountService.GetMemberModelFromMember(notification.CompletedBy);
            var enrichedProfile = _accountService.GetEnrichedProfile(memberModel);
            var badges = enrichedProfile?.Badges ?? Enumerable.Empty<BadgePage>();

            _badgeService.AddBadgeToMember(notification.CompletedBy, badges, new PerfectScoreBadge());
        }
    }
}