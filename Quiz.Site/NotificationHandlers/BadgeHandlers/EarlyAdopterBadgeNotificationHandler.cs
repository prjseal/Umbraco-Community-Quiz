using Quiz.Site.Models.Badges;
using Quiz.Site.Notifications.Member;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.NotificationHandlers.BadgeHandlers;

public class EarlyAdopterBadgeNotificationHandler : INotificationHandler<MemberRegisteredNotification>
{
    private readonly IBadgeService _badgeService;
    private readonly IAccountService _accountService;
    private readonly static DateTime EarlyAdopterThreshold = new(2022, 11, 5);

    public EarlyAdopterBadgeNotificationHandler(IBadgeService badgeService, IAccountService accountService)
    {
        _badgeService = badgeService;
        _accountService = accountService;
    }
    
    public void Handle(MemberRegisteredNotification notification)
    {
        if (DateTime.Now.Date < EarlyAdopterThreshold.Date)
        {
            var memberModel = _accountService.GetMemberModelFromMember(notification.RegisteredMember);
            var enrichedProfile = _accountService.GetEnrichedProfile(memberModel);
            var badges = enrichedProfile?.Badges ?? Enumerable.Empty<BadgePage>();

            _badgeService.AddBadgeToMember(notification.RegisteredMember, badges, new EarlyAdopterBadge());
        }
    }
}