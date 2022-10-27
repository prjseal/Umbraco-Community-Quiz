using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Notifications.Member
{
    public class MemberRegisteredNotification : INotification
    {
        public IMember RegisteredMember { get; }
        public IEnumerable<BadgePage> Badges {get;}

        public MemberRegisteredNotification(IMember registeredMember, IEnumerable<BadgePage> badges)
        {
            RegisteredMember = registeredMember;
            Badges = badges;
        }
    }
}