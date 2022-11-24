namespace Quiz.Site.Services;
public interface ILeaderboardService
{
    Task<IEnumerable<Models.PlayerRecord>> GetLeaderboardItems(string fallbackImageUrl);
}