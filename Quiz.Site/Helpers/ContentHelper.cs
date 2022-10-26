using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Helpers;

public static class ContentHelper
{
    public static List<BadgePage> GetBadges(IPublishedContentCache content)
    {
        var root = content.GetAtRoot().FirstOrDefault();
        if (root is not null)
        {
            var list =  root.FirstChild<BadgeListPage>()?.Children<BadgePage>()?.ToList();
            return list ?? new List<BadgePage>();
        }

        return new List<BadgePage>();
    }
}