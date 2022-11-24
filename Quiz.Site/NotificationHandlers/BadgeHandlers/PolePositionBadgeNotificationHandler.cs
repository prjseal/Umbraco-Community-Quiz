using Microsoft.Extensions.Caching.Memory;
using Quiz.Site.Enums;
using Quiz.Site.Models.Badges;
using Quiz.Site.Notifications.Quiz;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Events;

namespace Quiz.Site.NotificationHandlers.BadgeHandlers;

public class PolePositionBadgeNotificationHandler : INotificationHandler<QuizCompletedNotification>
{
    private readonly IBadgeService _badgeService;
    private readonly IQuizResultRepository _quizResultRepository;
    private readonly ILeaderboardService _leaderboardService;
    private readonly IMemoryCache _memoryCache;

    public PolePositionBadgeNotificationHandler(IBadgeService badgeService, IQuizResultRepository quizResultRepository, 
        ILeaderboardService leaderboardService, IMemoryCache memoryCache)
    {
        _badgeService = badgeService;
        _quizResultRepository = quizResultRepository;
        _leaderboardService = leaderboardService;
        _memoryCache = memoryCache;
    }

    public async void Handle(QuizCompletedNotification notification)
    {
        //TODO:Make the fallback image be dynamic
        var playerRecords = await _leaderboardService.GetLeaderboardItems("/media/jnsnuzn0/default-profile-image.png?width=100&height=100");
        if (playerRecords != null && playerRecords.Any())
        {
            var firstPerson = playerRecords.FirstOrDefault();
            if (firstPerson != null && firstPerson.MemberId == notification.CompletedBy.Id.ToString())
            {
                _badgeService.AddBadgeToMember(notification.CompletedBy, notification.Badges, new PolePositionBadge());
            }
        }

        if(playerRecords != null && playerRecords.Any())
        {
            _memoryCache.Set(CacheKey.LeaderBoard, playerRecords, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromHours(1),
                Priority = CacheItemPriority.High,
            });
        }
    }
}