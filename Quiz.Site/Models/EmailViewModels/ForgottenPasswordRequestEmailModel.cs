namespace Quiz.Site.Models.EmailViewModels
{
    public class ForgottenPasswordRequestEmailModel : EmailViewModelBase
    {        
        public ForgottenPasswordRequestEmailModel()
        {
            this.ViewPath = "/Views/EmailTemplates/ForgottenPasswordRequest.cshtml";
        }
        public string Token {get;set;}
        public string ForgottenPasswordPageUrl {get;set;}
        public override string ToPlainTextBody()
        {
            return $"Please visit the following URL in order to complete the password reset process: \n {ForgottenPasswordPageUrl}?t={Token}";
        }

    }
}