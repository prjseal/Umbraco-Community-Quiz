namespace Quiz.Site.Models.EmailViewModels
{
    public class AlreadyRegisteredNotificationModel : EmailViewModelBase
    {
        public AlreadyRegisteredNotificationModel()
        {
            RegistrationAttemptDateTime = DateTime.UtcNow;
            ViewPath = "/Views/EmailTemplates/AlreadyRegisteredNotification.cshtml";
        }
        public DateTime RegistrationAttemptDateTime {get; private set;}
        public override string ToPlainTextBody()
        {
            return $"Attempted registration occured at {RegistrationAttemptDateTime: yyyy'-'MM'-'dd HH':'mm'.'ss}";
        }
    }
}