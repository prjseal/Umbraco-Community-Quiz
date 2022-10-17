using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Quiz.Site.Enums;
using Quiz.Site.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "Leaderboard")]
    public class LeaderboardViewComponent : ViewComponent
    {
        private readonly IQuizResultRepository _quizResultRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IMemberManager _memberManager;

        public LeaderboardViewComponent(IQuizResultRepository quizResultRepository, IMemoryCache memoryCache, 
            IMemberManager memberManager)
        {
            _quizResultRepository = quizResultRepository;
            _memoryCache = memoryCache;
            _memberManager = memberManager;        
        }

        public async Task<IViewComponentResult> InvokeAsync(string fallbackImageUrl)
        {
            if (!_memoryCache.TryGetValue(CacheKey.LeaderBoard, out IEnumerable<Models.PlayerRecord> playerRecords))
            {
                playerRecords = _quizResultRepository.GetPlayerRecords();

                foreach (var record in playerRecords)
                {
                    var member = await _memberManager.FindByIdAsync(record.MemberId);
                    IPublishedContent memberContent = null;
                    record.Badges = 0;
                    record.AvatarUrl = fallbackImageUrl;
                    if (member != null)
                    {
                        var badgeCount = 0;
                        memberContent = _memberManager.AsPublishedMember(member);
                        if(memberContent != null)
                        {
                            record.Name = memberContent.Name;

                            if (memberContent.HasProperty("badges") && memberContent.HasValue("badges"))
                            {
                                var badges = memberContent.Value<IEnumerable<IPublishedContent>>("badges");
                                badgeCount = badges != null && badges.Any() ? badges.Count() : 0;
                            }

                            var avatarImage = memberContent.Value<MediaWithCrops>("avatar");
                            var avatarImageUrl = "";
                            if (avatarImage != null)
                            {
                                avatarImageUrl = avatarImage.GetCropUrl(50, 50);
                                record.AvatarUrl = avatarImageUrl;
                            }
                        }
                        record.Badges = badgeCount;
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

