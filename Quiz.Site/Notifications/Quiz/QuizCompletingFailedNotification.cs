using Umbraco.Cms.Core.Notifications;

namespace Quiz.Site.Notifications.Quiz;

public class QuizCompletingFailedNotification : INotification
{
    public string Reason { get; }
    
    public QuizCompletingFailedNotification(string reason)
    {
        Reason = reason;
    }
}