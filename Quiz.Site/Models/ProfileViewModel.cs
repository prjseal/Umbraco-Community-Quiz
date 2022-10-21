using GravatarHelper.Common;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Web.Common.PublishedModels;
using static Umbraco.Cms.Core.Constants.Conventions;

namespace Quiz.Site.Models
{
    public class ProfileViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<BadgePage> Badges { get; set; }
        public MediaWithCrops? Avatar { get; set; }

        internal string? GravatarUrl {
            get {
                var url = GravatarHelper.Common.GravatarHelper.CreateGravatarUrl(Email, 80, GravatarHelper.Common.GravatarHelper.DefaultImageIdenticon, GravatarRating.G, true, true, true);
                return url;
            }
        }

        public string? AvatarUrl {
            get {
                if (Avatar == null)
                {
                    return GravatarUrl;
                }

                return Avatar.GetCropUrl(100, 100);
            }
        }
    }
}
