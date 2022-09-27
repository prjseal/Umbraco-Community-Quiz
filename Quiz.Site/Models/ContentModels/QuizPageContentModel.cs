using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Quiz.Site.Models.ContentModels;

public class QuizPageContentModel : ContentModel
{
    public QuizPageContentModel(IPublishedContent? content) : base(content)
    {
    }

    public QuizViewModel Quiz { get; set; }
}
