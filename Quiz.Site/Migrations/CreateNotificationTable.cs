using Quiz.Site.Models;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Quiz.Site.Migrations
{
    /// <summary>
    ///  creates a table in the DB if it's not already there
    /// </summary>
    public class CreateNotificationTable : MigrationBase
    {
        public CreateNotificationTable(IMigrationContext context)
            : base(context)
        {
        }

        protected override void Migrate()
        {
            if (!TableExists(Quiz.Tables.NotificationTable))
                Create.Table<Notification>().Do();
        }
    }
}