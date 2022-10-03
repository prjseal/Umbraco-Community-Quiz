using Quiz.Site.Models;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Quiz.Site.Migrations
{
    /// <summary>
    ///  creates a table in the DB if it's not already there
    /// </summary>
    public class CreateReadNotificationTable : MigrationBase
    {
        public CreateReadNotificationTable(IMigrationContext context)
            : base(context)
        {
        }

        protected override void Migrate()
        {
            if (!TableExists(Quiz.Tables.ReadNotificationTable))
                Create.Table<ReadNotification>().Do();
        }
    }
}