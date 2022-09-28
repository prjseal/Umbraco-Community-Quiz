using Konstrukt.Configuration.Actions;
using Konstrukt.Configuration.Builders;
using Konstrukt.Persistence;
using Quiz.Site.Enums;
using Quiz.Site.Models;
using Quiz.Site.ValueMappers;

namespace Quiz.Site.Actions;
public class ChangeStatusAction : KonstruktAction<ChangeStatusSettings, KonstruktActionResult>
{
    private readonly IKonstruktRepositoryFactory _repoFactory;

    public override string Icon => "icon-nodes";
    public override string Alias => "changestatus";
    public override string Name => "Change Status";

    public ChangeStatusAction(IKonstruktRepositoryFactory repoFactory)
    {
        _repoFactory = repoFactory;
    }

    public override void Configure(KonstruktSettingsConfigBuilder<ChangeStatusSettings> settingsConfig)
    {
        settingsConfig.AddFieldset("General", fieldsetConfig => fieldsetConfig
           .AddField(s => s.Status).SetDataType("[DataList] Question Status").SetValueMapper<EnumDropdownValueMapper<QuestionStatus>>()
       );
    }

    public override bool IsVisible(KonstruktActionVisibilityContext ctx)
    {
        return ctx.ActionType == KonstruktActionType.Bulk
            || ctx.ActionType == KonstruktActionType.Row;
    }

    public override KonstruktActionResult Execute(string collectionAlias, object[] entityIds, ChangeStatusSettings settings)
    {
        try
        {
            var repo = _repoFactory.GetRepository<Question, int>(collectionAlias);

            var ids = entityIds.Select(x => int.Parse(x?.ToString())).ToArray();
            var result = repo.GetAll(x => ids.Contains(x.Id));

            if (result.Success)
            {
                foreach (var entity in result.Model)
                {
                    entity.Status = ((int)settings.Status).ToString();

                    repo.Save(entity);
                }
            }

            return new KonstruktActionResult(true);
        }
        catch (Exception ex)
        {
            return new KonstruktActionResult(false, new KonstruktActionNotification("Failed to update status", ex.Message));
        }
    }

}

public class ChangeStatusSettings
{
    public QuestionStatus Status { get; set; }
}
