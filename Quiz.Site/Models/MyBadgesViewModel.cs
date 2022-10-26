using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Models;

public class MyBadgesViewModel
{
    public IEnumerable<BadgePage> Badges { get; set; }
}