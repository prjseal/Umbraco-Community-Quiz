using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;

namespace Quiz.Site.Notifications;

public class QuizCompletedNotification : INotification
{
    public IMember Member { get; }
    
    public int QuizTotal { get; }

    public int QuizScore { get; }
    

    public QuizCompletedNotification(IMember member, int quizTotal, int quizScore)
    {
        Member = member;
        QuizTotal = quizTotal;
        QuizScore = quizScore;
    }
}