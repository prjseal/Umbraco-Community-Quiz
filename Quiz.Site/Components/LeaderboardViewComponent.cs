using Microsoft.AspNetCore.Mvc;
using Quiz.Site.Services;

namespace Quiz.Site.Components
{
    [ViewComponent(Name = "Leaderboard")]
    public class LeaderboardViewComponent : ViewComponent
    {
        private readonly IQuizResultRepository _quizResultRepository;

        public LeaderboardViewComponent(IQuizResultRepository quizResultRepository)
        {
            _quizResultRepository = quizResultRepository;
        }

        public IViewComponentResult Invoke()
        {
            var playerRecords = _quizResultRepository.GetPlayerRecords();

            playerRecords = playerRecords.OrderByDescending(x => x.Correct).ThenByDescending(y => y.Total).ThenBy(z => z.Quizzes);

            return View(playerRecords);
        }
    }
}
