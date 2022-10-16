using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;

namespace Quiz.Site.Notifications;

public class MemberRegisteringFailedNotification : INotification
{
    public string Reason { get; }

    public MemberRegisteringFailedNotification(string reason)
    {
        Reason = reason;
    }
}