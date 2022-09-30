using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Quiz.Site.Services;
public interface IBadgeService
{
    bool AddBadgeToMember(IMember member, IPublishedContent contentItem);
}
