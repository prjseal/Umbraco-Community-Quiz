using Umbraco.Cms.Core.Notifications;

namespace Quiz.Site.Notifications.Question;

public class QuestionCreatingFailedNotification : INotification
{
    public string Reason { get; }
    
    public QuestionCreatingFailedNotification(string reason)
    {
        Reason = reason;
    }
}