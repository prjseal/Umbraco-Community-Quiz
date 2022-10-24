using Quiz.Site.NotificationHandlers.BadgeHandlers;
using Quiz.Site.Notifications.Member;
using Quiz.Site.Notifications.Profile;
using Quiz.Site.Notifications.Question;
using Quiz.Site.Notifications.Quiz;
using Umbraco.Cms.Core.Composing;

namespace Quiz.Site.Composing;

public class RegisterNotificationsComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        //badges
        builder.AddNotificationHandler<MemberRegisteredNotification, EarlyAdopterBadgeNotificationHandler>();
        builder.AddNotificationHandler<QuizCompletedNotification, PerfectScoreBadgeNotificationHandler>();
        builder.AddNotificationHandler<QuestionCreatedNotification, TeacherBadgeNotificationHandler>();
        builder.AddNotificationHandler<ProfileUpdatedNotification, UpdatedProfileBadgeNotificationHandler>();
    }
}
