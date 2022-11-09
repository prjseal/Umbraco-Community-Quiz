using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Quiz.Site.Models.ContentModels;

public class ProfilePageContentModel : ContentModel
{
    public ProfilePageContentModel(IPublishedContent? content) : base(content)
    {
    }

    public ProfileResultsViewModel ProfileResults { get; set; }
}
