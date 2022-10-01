using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quiz.Site.Extensions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Services;
public class BadgeService : IBadgeService
{
    private readonly IMemberService _memberService;
    private readonly IUmbracoContextFactory _umbracoContextFactory;

    public BadgeService(IMemberService memberService, IUmbracoContextFactory umbracoContextFactory)
    {
        _memberService = memberService;
        _umbracoContextFactory = umbracoContextFactory;
    }

    public IPublishedContent GetBadgeByName(string badgeName)
    {
        IPublishedContent badge = null;
        using (var umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext())
        {
            var contentQuery = umbracoContextReference.UmbracoContext.Content; 
            var homePage = contentQuery.GetAtRoot().FirstOrDefault();
            if (homePage == null) return null;
            var badgeList = homePage?.FirstChildOfType(BadgeListPage.ModelTypeAlias) ?? null;
            if (badgeList == null && badgeList?.Children != null ) return null;
            badge = badgeList?.Children?.FirstOrDefault(x => string.Equals(x.Name, badgeName, StringComparison.CurrentCultureIgnoreCase)) ?? null;
        }
        return badge;
    }

    public bool HasBadge(Umbraco.Cms.Web.Common.PublishedModels.Member member, IPublishedContent contentItem)
    {
        var udi = contentItem.GetUdiObject().ToString();

        if (member == null) throw new Exception("Member is null");

        var badgeUdis = new List<string>();

        if(member.Badges != null && member.Badges.Any())
        {
            foreach(var badge in member.Badges)
            {
                badgeUdis.Add(badge.GetUdiObject().ToString());
            }
        }

        return badgeUdis?.Contains(udi) ?? false;
    }

    public bool AddBadgeToMember(IMember member, IPublishedContent contentItem)
    {
        var badgesValue = member.GetValue<string>("badges");

        JArray badgesArray = null;

        if (!string.IsNullOrWhiteSpace(badgesValue))
        {
            badgesArray = JsonConvert.DeserializeObject<JArray>(badgesValue);
        }
        else
        {
            badgesArray = new JArray();
        }

        badgesArray.Add(contentItem.GetUdiObject().ToString());

        member.SetValue("badges", badgesArray);

        _memberService.Save(member);

        return true;
    }
}
