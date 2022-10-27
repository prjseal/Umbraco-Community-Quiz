using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quiz.Site.Extensions;
using Quiz.Site.Helpers;
using Quiz.Site.Models.Badges;
using Quiz.Site.Notifications.Badge;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Services;
public class BadgeService : IBadgeService
{
    private readonly IMemberService _memberService;
    private readonly IUmbracoContextFactory _umbracoContextFactory;
    private readonly IEventAggregator _eventAggregator;

    public BadgeService(IMemberService memberService, IEventAggregator eventAggregator, IUmbracoContextFactory umbracoContextFactory)
    {
        _memberService = memberService;
        _eventAggregator = eventAggregator;
        _umbracoContextFactory = umbracoContextFactory;
    }
    
    public BadgePage? GetBadgeByName(string name)
    {
        using var umbracoContextReference = _umbracoContextFactory.EnsureUmbracoContext();
        var badges = ContentHelper.GetBadges(umbracoContextReference.UmbracoContext.Content!);
        return badges.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
    }

    public bool HasBadge(IMember member, IEnumerable<BadgePage> badges, BadgePage badge)
    {
        if (member == null)
        {
            throw new Exception("Member is null");
        }

        var badgeIds = badges?.Select(x => x.Id) ?? Enumerable.Empty<int>();
        return badgeIds != null && badgeIds.Contains(badge.Id);
    }
    

    public bool AddBadgeToMember(IMember member, IEnumerable<BadgePage> badges, IBadge badge, bool pushNotification = true)
    {
       var success =  AssignBadgeToMember(member, badges, badge);
       if (success && pushNotification)
       {
           _eventAggregator.Publish(new BadgeAssignedNotification(badge, member));
       }

       return success;
    }

    private bool AssignBadgeToMember(IMember member, IEnumerable<BadgePage> badges, IBadge badge)
    {
        var badgeItem = GetBadgeByName(badge.UniqueUmbracoName);
        if (badgeItem is null)
        {
            return false;
        }

        if (HasBadge(member, badges, badgeItem))
        {
            return false;
        }

        var badgesValue = member.GetValue<string>("badges");
        var badgesArray = !string.IsNullOrWhiteSpace(badgesValue) ? JsonConvert.DeserializeObject<JArray>(badgesValue) : new JArray();;
            
        badgesArray?.Add(badgeItem.GetUdiObject().ToString());
        member.SetValue("badges", badgesArray);
        
        _memberService.Save(member);
        
        return true;

    }
}
