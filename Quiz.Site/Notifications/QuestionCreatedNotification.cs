using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Notifications;

namespace Quiz.Site.Notifications;

public class QuestionCreatedNotification : INotification
{
    public IMember CreatedBy { get; }

    public QuestionCreatedNotification(IMember createdBy)
    {
        CreatedBy = createdBy;
    }
}