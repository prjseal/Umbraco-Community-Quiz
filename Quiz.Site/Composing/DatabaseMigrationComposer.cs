using Quiz.Site.Migrations;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Migrations;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Scoping;

namespace Quiz.Site.Composing
{
    public class DatabaseMigrationComposer : IUserComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddNotificationHandler<UmbracoApplicationStartingNotification, DatabaseMigrationAppStartingHandler>();
        }
    }

    public class DatabaseMigrationAppStartingHandler : INotificationHandler<UmbracoApplicationStartingNotification>
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;

        public DatabaseMigrationAppStartingHandler(IScopeProvider scopeProvider,
            IMigrationPlanExecutor migrationPlanExecutor,
            IKeyValueService keyValueService,
            IRuntimeState runtimeState)
        {
            _scopeProvider = scopeProvider;
            _migrationPlanExecutor = migrationPlanExecutor;
            _keyValueService = keyValueService;
            _runtimeState = runtimeState;
        }

            public void Handle(UmbracoApplicationStartingNotification notification)
        {
            if (notification.RuntimeLevel >= RuntimeLevel.Run)
            {
                var plan = new MigrationPlan("QuizApplication");

                plan.From(string.Empty)
                    .To<CreateQuestionTable>("1.0.0")
                    .To<CreateQuizResultTable>("1.0.1")
                    .To<CreateNotificationTable>("1.0.2")
                    .To<CreateReadNotificationTable>("1.0.3");

                var upgrader = new Upgrader(plan);

                upgrader.Execute(_migrationPlanExecutor, _scopeProvider, _keyValueService);
            }
        }
    }
}
