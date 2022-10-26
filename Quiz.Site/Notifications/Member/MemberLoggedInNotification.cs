using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Notifications;

public class MemberLoggedInNotification : INotification
{
    public IMember LoggedInMember { get; }
    public IEnumerable<BadgePage> Badges { get; }

    public MemberLoggedInNotification(IMember loggedInMember, IEnumerable<BadgePage> badges)
    {
        LoggedInMember = loggedInMember;
        Badges = badges;
    }
}