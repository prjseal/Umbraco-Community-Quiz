using Quiz.Site.Models.Badges;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;

namespace Quiz.Site.Notifications.Badge;

public class BadgeAssignedNotification : INotification
{
    public IBadge Badge { get; }

    public IMember AssignedTo { get; }
    
    public BadgeAssignedNotification(IBadge badge, IMember assignedTo)
    {
        Badge = badge;
        AssignedTo = assignedTo;
    }
}