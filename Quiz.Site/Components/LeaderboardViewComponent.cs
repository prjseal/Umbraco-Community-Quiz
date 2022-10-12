using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Quiz.Site.Enums;
using Quiz.Site.Services;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "Leaderboard")]
    public class LeaderboardViewComponent : ViewComponent
    {
        private readonly IQuizResultRepository _quizResultRepository;
        private readonly IMemoryCache _memoryCache;

        public LeaderboardViewComponent(IQuizResultRepository quizResultRepository, IMemoryCache memoryCache)
        {
            _quizResultRepository = quizResultRepository;
            _memoryCache = memoryCache;
        }

        public IViewComponentResult Invoke()
        {
            if (!_memoryCache.TryGetValue(CacheKey.LeaderBoard, out IEnumerable<Models.PlayerRecord> playerRecords))
            {
                playerRecords = _quizResultRepository.GetPlayerRecords();
                playerRecords = playerRecords.OrderByDescending(x => x.Correct).ThenByDescending(y => y.Total).ThenBy(z => z.Quizzes);

                _memoryCache.Set(CacheKey.LeaderBoard, playerRecords, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromDays(1),
                    Priority = CacheItemPriority.High,
                });
            }

            return View(playerRecords);
        }
    }
}

