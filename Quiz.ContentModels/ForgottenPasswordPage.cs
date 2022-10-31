namespace Umbraco.Cms.Web.Common.PublishedModels
{
    public partial class ForgottenPasswordPage
    {
        public bool ShowPasswordResetForm {get;set;}
        public string CurrentToken {get;set;}
        public string EmailAddress {get;set;}
    }
}