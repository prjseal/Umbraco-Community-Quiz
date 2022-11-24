using Microsoft.Extensions.Caching.Memory;
using Quiz.Site.Enums;
using Umbraco.Cms.Core.Security;

namespace Quiz.Site.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly IQuizResultRepository _quizResultRepository;
    private readonly IMemoryCache _memoryCache;
    private readonly IMemberManager _memberManager;
    private readonly IAccountService _accountService;

    public LeaderboardService(IQuizResultRepository quizResultRepository, IMemoryCache memoryCache,
    IMemberManager memberManager, IAccountService accountService)
    {
        _quizResultRepository = quizResultRepository;
        _memoryCache = memoryCache;
        _memberManager = memberManager;
        _accountService = accountService;
    }

    public async Task<IEnumerable<Models.PlayerRecord>> GetLeaderboardItems(string fallbackImageUrl)
    {
        IEnumerable<Models.PlayerRecord> playerRecords = _quizResultRepository.GetPlayerRecords();

        foreach (var record in playerRecords)
        {
            var member = await _memberManager.FindByIdAsync(record.MemberId);

            if (member != null)
            {
                var memberContent = _memberManager.AsPublishedMember(member);
                if (memberContent == null) continue;

                var enrichedProfile = _accountService.GetEnrichedProfile(memberContent);
                if (enrichedProfile == null) continue;

                if (enrichedProfile.HideProfile) continue;

                record.Badges = enrichedProfile.Badges?.Count() ?? 0;
                record.AvatarUrl = enrichedProfile.Avatar?.GetCropUrl(50, 50) ?? fallbackImageUrl;
                record.Name = enrichedProfile.Name;
            }
        }

        playerRecords = playerRecords.Where(x => !string.IsNullOrWhiteSpace(x.Name))
                                    .OrderByDescending(x => x.Correct)
                                    .ThenByDescending(x => x.Total)
                                    .ThenBy(x => x.Quizzes)
                                    .ThenByDescending(x => x.Badges)
                                    .ThenBy(x => x.DateOfLastQuiz);

        return playerRecords;
    }
}