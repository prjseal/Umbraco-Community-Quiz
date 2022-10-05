using Quiz.Site.Services;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Community.Contentment.DataEditors;

namespace Quiz.Site.DataListSources;

public class QuestionDataSource
{
    public class TimeZoneDataSource : IDataListSource
    {
        private readonly IQuestionRepository _questionRepository;

        public TimeZoneDataSource(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public string Name => "Questions";

        public string Description => "Data source for all approved questions.";

        public string Icon => "icon-help-alt";

        public OverlaySize OverlaySize => OverlaySize.Small;

        public Dictionary<string, object> DefaultValues => default;

        public IEnumerable<ConfigurationField> Fields => default;

        public string Group => "Custom";

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var items = new List<DataListItem>();

            var questions = _questionRepository.GetAllByStatus(Enums.QuestionStatus.Approved);

            if(questions != null && questions.Any())
            {
                foreach(var item in questions)
                {
                    items.Add(new DataListItem
                    {
                        Name = item.QuestionText,
                        Value = item.Id.ToString()
                    });
                }
            }

            return items;
        }
    }
}
