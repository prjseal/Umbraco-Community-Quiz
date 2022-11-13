using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Notifications.Quiz;

public class QuizCompletedNotification : INotification
{
    public IMember CompletedBy { get; }
    
    public int QuizTotal { get; }

    public int QuizScore { get; }
    public IEnumerable<BadgePage> Badges { get; }

    public string CompletedQuizUdi { get; set; }

    public QuizCompletedNotification(IMember completedBy, int quizTotal, int quizScore, IEnumerable<BadgePage> badges, string completedQuizUdi)
    {
        CompletedBy = completedBy;
        QuizTotal = quizTotal;
        QuizScore = quizScore;
        Badges = badges;
        CompletedQuizUdi = completedQuizUdi;
    }
}