using Quiz.Site.Models.Badges;
using Quiz.Site.Notifications.Quiz;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.NotificationHandlers.BadgeHandlers;

public class UmbracoNinjaBadgeNotificationHandler : INotificationHandler<QuizCompletedNotification>
{
    private readonly IBadgeService _badgeService;
    private readonly IQuizResultRepository _quizResultRepository;
    private readonly IAccountService _accountService;

    public UmbracoNinjaBadgeNotificationHandler(IBadgeService badgeService, IQuizResultRepository quizResultRepository,
        IAccountService accountService)
    {
        _badgeService = badgeService;
        _quizResultRepository = quizResultRepository;
        _accountService = accountService;
    }

    public void Handle(QuizCompletedNotification notification)
    {
        if (notification.QuizScore >= notification.QuizTotal)
        {
            var allResults = _quizResultRepository.GetAllByMemberId(notification.CompletedBy.Id);

            if (allResults == null || !allResults.Any() || allResults.Count() < 10) return;

            var last10results = allResults.OrderByDescending(r => r.DateCreated).Take(10);

            var sumOfScore = last10results.Sum(x => x.Score);
            var sumOfTotal = last10results.Sum(x => x.Total);

            if (sumOfScore < sumOfTotal) return;

            var memberModel = _accountService.GetMemberModelFromMember(notification.CompletedBy);
            var enrichedProfile = _accountService.GetEnrichedProfile(memberModel);
            var badges = enrichedProfile?.Badges ?? Enumerable.Empty<BadgePage>();

            _badgeService.AddBadgeToMember(notification.CompletedBy, badges, new UmbracoNinjaBadge());
        }
    }
}