using Quiz.Site.Models.Badges;
using Quiz.Site.Notifications.Quiz;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Events;

namespace Quiz.Site.NotificationHandlers.BadgeHandlers;

public class EarlyBirdBadgeNotificationHandler : INotificationHandler<QuizCompletedNotification>
{
    private readonly IBadgeService _badgeService;
    private readonly IQuizResultRepository _quizResultRepository;

    public EarlyBirdBadgeNotificationHandler(IBadgeService badgeService, IQuizResultRepository quizResultRepository)
    {
        _badgeService = badgeService;
        _quizResultRepository = quizResultRepository;
    }

    public void Handle(QuizCompletedNotification notification)
    {
        var firstToCompleteThisQuiz = _quizResultRepository.GetFirstResultForThisQuiz(notification.CompletedQuizUdi);
        if(firstToCompleteThisQuiz != null && notification.CompletedBy?.Id == firstToCompleteThisQuiz.MemberId)
        {
            _badgeService.AddBadgeToMember(notification.CompletedBy, notification.Badges, new EarlyBirdBadge());
        }
    }
}