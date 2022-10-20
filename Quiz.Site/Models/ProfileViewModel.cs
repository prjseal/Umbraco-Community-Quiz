using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Web.Common.PublishedModels;

namespace Quiz.Site.Models
{
    public class ProfileViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<BadgePage> Badges { get; set; }
        public MediaWithCrops Avatar { get; set; }
    }
}
