using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Quiz.Site.Enums;
using Quiz.Site.Services;
namespace Quiz.Site.Components
{
    [ViewComponent(Name = "Leaderboard")]
    public class LeaderboardViewComponent : ViewComponent
    {
        private readonly ILeaderboardService _leaderboardService;
        private readonly IMemoryCache _memoryCache;

        public LeaderboardViewComponent(ILeaderboardService leaderboardService, IMemoryCache memoryCache)
        {
            _leaderboardService = leaderboardService;
            _memoryCache = memoryCache;
        }

        public async Task<IViewComponentResult> InvokeAsync(string fallbackImageUrl)
        {
            if (!_memoryCache.TryGetValue(CacheKey.LeaderBoard, out IEnumerable<Models.PlayerRecord> playerRecords))
            {
                playerRecords = await _leaderboardService.GetLeaderboardItems(fallbackImageUrl);

                _memoryCache.Set(CacheKey.LeaderBoard, playerRecords, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromHours(1),
                    Priority = CacheItemPriority.High,
                }); 
            }

            return View(playerRecords);
        }
    }
}

