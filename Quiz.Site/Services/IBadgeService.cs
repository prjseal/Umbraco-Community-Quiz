﻿using Quiz.Site.Models.Badges;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Services;
public interface IBadgeService
{
    BadgePage? GetBadgeByName(string badgeName);
    bool HasBadge(IMember member, IEnumerable<BadgePage> badges, BadgePage badge);
    bool AddBadgeToMember(IMember member, IEnumerable<BadgePage> badges, IBadge badge, bool pushNotification = true);
}
