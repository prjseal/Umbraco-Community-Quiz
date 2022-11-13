using Quiz.Site.Models.Badges;
using Quiz.Site.Notifications.Quiz;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Events;

namespace Quiz.Site.NotificationHandlers.BadgeHandlers;

public class HighFiveYouRockBadgeNotificationHandler : INotificationHandler<QuizCompletedNotification>
{
    private readonly IBadgeService _badgeService;
    private readonly IQuizResultRepository _quizResultRepository;

    public HighFiveYouRockBadgeNotificationHandler(IBadgeService badgeService, IQuizResultRepository quizResultRepository)
    {
        _badgeService = badgeService;
        _quizResultRepository = quizResultRepository;
    }

    public void Handle(QuizCompletedNotification notification)
    {
        if (notification.QuizScore >= notification.QuizTotal)
        {
            var allResults = _quizResultRepository.GetAllByMemberId(notification.CompletedBy.Id);

            if (allResults == null || !allResults.Any() || allResults.Count() < 5) return;

            var last5results = allResults.OrderByDescending(r => r.DateCreated).Take(5);

            var sumOfScore = last5results.Sum(x => x.Score);
            var sumOfTotal = last5results.Sum(x => x.Total);

            if (sumOfScore < sumOfTotal) return;

            _badgeService.AddBadgeToMember(notification.CompletedBy, notification.Badges, new HighFiveYouRockBadge());
        }
    }
}