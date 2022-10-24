using Quiz.Site.Models.Badges;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Services;
public interface IBadgeService
{
    BadgePage? GetBadgeByName(string badgeName);
    bool HasBadge(IMember member, BadgePage badge);
    
    bool AddBadgeToMember(IMember member, IBadge badge, bool checkCondition = true);
}
