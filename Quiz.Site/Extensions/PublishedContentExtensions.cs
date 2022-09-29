using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Quiz.Site.Extensions;

public static class PublishedContentExtensions
{
    public static Udi GetUdiObject(this IPublishedContent contentItem)
    {
        var key = contentItem.Key;
        Udi udi = null;
        switch(contentItem.ItemType)
        {
            case PublishedItemType.Content:
                udi = Udi.Create("document", key);
                break;
            case PublishedItemType.Media:
                udi = Udi.Create("media", key);
                break;
            case PublishedItemType.Member:
                udi = Udi.Create("member", key);
                break;
        }
        return udi;
    }
}
