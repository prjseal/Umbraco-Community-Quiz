using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Quiz.Site.Services;
public interface IBadgeService
{
    IPublishedContent GetBadgeByName(string badgeName);
    bool HasBadge(Umbraco.Cms.Web.Common.PublishedModels.Member member, IPublishedContent contentItem);
    bool AddBadgeToMember(IMember member, IPublishedContent contentItem);
}
