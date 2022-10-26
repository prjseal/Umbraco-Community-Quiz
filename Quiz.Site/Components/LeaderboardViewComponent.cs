using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Quiz.Site.Enums;
using Quiz.Site.Services;
using Umbraco.Cms.Core.Security;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "Leaderboard")]
    public class LeaderboardViewComponent : ViewComponent
    {
        private readonly IQuizResultRepository _quizResultRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IMemberManager _memberManager;
        private readonly IAccountService _accountService;

        public LeaderboardViewComponent(IQuizResultRepository quizResultRepository, IMemoryCache memoryCache, 
            IMemberManager memberManager, IAccountService accountService)
        {
            _quizResultRepository = quizResultRepository;
            _memoryCache = memoryCache;
            _memberManager = memberManager;        
            _accountService = accountService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string fallbackImageUrl)
        {
            if (!_memoryCache.TryGetValue(CacheKey.LeaderBoard, out IEnumerable<Models.PlayerRecord> playerRecords))
            {
                playerRecords = _quizResultRepository.GetPlayerRecords();

                foreach (var record in playerRecords)
                {
                    var member = await _memberManager.FindByIdAsync(record.MemberId);
                    
                    if (member != null)
                    {
                        var memberContent = _memberManager.AsPublishedMember(member);
                        if (memberContent == null) continue;

                        var enrichedProfile = _accountService.GetEnrichedProfile(memberContent);
                        if (enrichedProfile == null) continue;

                        record.Badges = enrichedProfile.Badges?.Count() ?? 0;
                        record.AvatarUrl = enrichedProfile.Avatar?.GetCropUrl(50, 50) ?? fallbackImageUrl;
                        record.Name = enrichedProfile.Name;
                    }
                }

                playerRecords = playerRecords.OrderByDescending(x => x.Correct)
                                            .ThenByDescending(x => x.Total)
                                            .ThenBy(x => x.Quizzes)
                                            .ThenByDescending(x => x.Badges)
                                            .ThenBy(x => x.DateOfLastQuiz);

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

