namespace Quiz.Site.Models
{
    public abstract class EmailViewModelBase
    {
        public abstract string ToPlainTextBody();

        public virtual string ViewPath { get; set; }

        public virtual string SiteDomain { get; set; }
    }
}