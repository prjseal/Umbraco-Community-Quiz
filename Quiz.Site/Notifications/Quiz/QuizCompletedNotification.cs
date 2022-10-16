using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;

namespace Quiz.Site.Notifications.Quiz;

public class QuizCompletedNotification : INotification
{
    public IMember CompletedBy { get; }
    
    public int QuizTotal { get; }

    public int QuizScore { get; }

    public QuizCompletedNotification(IMember completedBy, int quizTotal, int quizScore)
    {
        CompletedBy = completedBy;
        QuizTotal = quizTotal;
        QuizScore = quizScore;
    }
}