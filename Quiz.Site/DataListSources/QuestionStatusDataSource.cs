using Quiz.Site.Enums;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Community.Contentment.DataEditors;

namespace Quiz.Site.DataListSources;

public class QuestionStatusDataSource
{
    public class TimeZoneDataSource : IDataListSource
    {
        public string Name => "Question Statuses";

        public string Description => "Data source for all question statuses.";

        public string Icon => "icon-flag";

        public OverlaySize OverlaySize => OverlaySize.Small;

        public Dictionary<string, object> DefaultValues => default;

        public IEnumerable<ConfigurationField> Fields => default;

        public string Group => "Custom";

        public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
        {
            var items = new List<DataListItem>();

            items.Add(new DataListItem
            {
                Name = Enum.GetName(QuestionStatus.Approved),
                Value = ((int)QuestionStatus.Approved).ToString()
            });
            items.Add(new DataListItem
            {
                Name = Enum.GetName(QuestionStatus.Deleted),
                Value = ((int)QuestionStatus.Deleted).ToString()
            });
            items.Add(new DataListItem
            {
                Name = Enum.GetName(QuestionStatus.Incorrect),
                Value = ((int)QuestionStatus.Incorrect).ToString()
            });
            items.Add(new DataListItem
            {
                Name = Enum.GetName(QuestionStatus.Pending),
                Value = ((int)QuestionStatus.Pending).ToString()
            });
            items.Add(new DataListItem
            {
                Name = Enum.GetName(QuestionStatus.Used),
                Value = ((int)QuestionStatus.Used).ToString()
            });

            return items;
        }
    }
}
