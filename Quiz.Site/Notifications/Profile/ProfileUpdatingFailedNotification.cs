using Umbraco.Cms.Core.Notifications;

namespace Quiz.Site.Notifications.Profile;

public class ProfileUpdatingFailedNotification : INotification
{
    public string Reason { get; }

    public ProfileUpdatingFailedNotification(string reason)
    {
        Reason = reason;
    }
}