using Quiz.Site.Models.Badges;
using Quiz.Site.Notifications.Quiz;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.NotificationHandlers.BadgeHandlers;

public class EarlyBirdBadgeNotificationHandler : INotificationHandler<QuizCompletedNotification>
{
    private readonly IBadgeService _badgeService;
    private readonly IQuizResultRepository _quizResultRepository;
    private readonly IAccountService _accountService;

    public EarlyBirdBadgeNotificationHandler(IBadgeService badgeService, IQuizResultRepository quizResultRepository,
        IAccountService accountService)
    {
        _badgeService = badgeService;
        _quizResultRepository = quizResultRepository;
        _accountService = accountService;
    }

    public void Handle(QuizCompletedNotification notification)
    {
        var firstToCompleteThisQuiz = _quizResultRepository.GetFirstResultForThisQuiz(notification.CompletedQuizUdi);
        if(firstToCompleteThisQuiz != null && notification.CompletedBy?.Id == firstToCompleteThisQuiz.MemberId)
        {
            var memberModel = _accountService.GetMemberModelFromMember(notification.CompletedBy);
            var enrichedProfile = _accountService.GetEnrichedProfile(memberModel);
            var badges = enrichedProfile?.Badges ?? Enumerable.Empty<BadgePage>();

            _badgeService.AddBadgeToMember(notification.CompletedBy, badges, new EarlyBirdBadge());
        }
    }
}