using Quiz.Site.Models;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Quiz.Site.Migrations
{
    /// <summary>
    ///  creates a table in the DB if it's not already there
    /// </summary>
    public class CreateQuizResultTable : MigrationBase
    {
        public CreateQuizResultTable(IMigrationContext context)
            : base(context)
        {
        }

        protected override void Migrate()
        {
            if (!TableExists(Quiz.Tables.QuizResultTable))
                Create.Table<QuizResult>().Do();
        }
    }
}