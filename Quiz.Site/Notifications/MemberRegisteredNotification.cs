using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;

namespace Quiz.Site.Notifications;

public class MemberRegisteredNotification : INotification
{
    public IMember RegisteredMember { get; }

    public MemberRegisteredNotification(IMember registeredMember)
    {
        RegisteredMember = registeredMember;
    }
}