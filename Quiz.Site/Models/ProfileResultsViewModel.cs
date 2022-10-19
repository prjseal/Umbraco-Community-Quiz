using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Models;

public class ProfileResultsViewModel
{
    public Member Member { get; set; }

    public ProfileViewModel Profile { get; set; }

    public PlayerRecord PlayerRecord { get; set; }
}
