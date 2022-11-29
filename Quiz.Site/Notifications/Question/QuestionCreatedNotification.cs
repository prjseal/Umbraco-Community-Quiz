using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Notifications.Question;

public class QuestionCreatedNotification : INotification
{
    public IMember CreatedBy { get; }

    public QuestionCreatedNotification(IMember createdBy)
    {
        CreatedBy = createdBy;
    }
}