using Quiz.Site.Models.Badges;
using Quiz.Site.Notifications.Profile;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.NotificationHandlers.BadgeHandlers;

public class UpdatedProfileBadgeNotificationHandler : INotificationHandler<ProfileUpdatedNotification>
{
    private readonly IBadgeService _badgeService;
    private readonly IAccountService _accountService;

    public UpdatedProfileBadgeNotificationHandler(IBadgeService badgeService, IAccountService accountService)
    {
        _badgeService = badgeService;
        _accountService = accountService;
    }

    public void Handle(ProfileUpdatedNotification notification)
    {
        var memberModel = _accountService.GetMemberModelFromMember(notification.UpdatedBy);
        var enrichedProfile = _accountService.GetEnrichedProfile(memberModel);
        var badges = enrichedProfile?.Badges ?? Enumerable.Empty<BadgePage>();

        _badgeService.AddBadgeToMember(notification.UpdatedBy, badges, new UpdatedProfileBadge());
    }
}