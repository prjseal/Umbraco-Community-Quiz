using Quiz.Site.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Notifications.Profile;

public class ProfileUpdatedNotification : INotification
{
    public IMember UpdatedBy { get; }
    public ProfileViewModel? ProfileViewModel { get; }
    public IEnumerable<BadgePage> Badges { get; }

    public ProfileUpdatedNotification(IMember updatedBy, ProfileViewModel profile, IEnumerable<BadgePage> badges)
    {
        UpdatedBy = updatedBy;
        ProfileViewModel = profile;
        Badges = badges;
    }
}