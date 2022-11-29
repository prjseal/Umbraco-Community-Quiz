using Quiz.Site.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Notifications.Profile;

public class ProfileUpdatedNotification : INotification
{
    public IMember UpdatedBy { get; }

    public ProfileUpdatedNotification(IMember updatedBy)
    {
        UpdatedBy = updatedBy;
    }
}