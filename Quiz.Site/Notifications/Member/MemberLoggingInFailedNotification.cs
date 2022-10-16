using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;

namespace Quiz.Site.Notifications;

public class MemberLoggingInFailedNotification : INotification
{
    public string Reason { get; }

    public MemberLoggingInFailedNotification(string reason)
    {
        Reason = reason;
    }
}