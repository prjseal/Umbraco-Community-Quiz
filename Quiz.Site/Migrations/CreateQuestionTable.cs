using Quiz.Site.Models;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Quiz.Site.Migrations
{
    /// <summary>
    ///  creates a table in the DB if it's not already there
    /// </summary>
    public class CreateQuestionTable : MigrationBase
    {
        public CreateQuestionTable(IMigrationContext context)
            : base(context)
        {
        }

        protected override void Migrate()
        {
            if (!TableExists(Quiz.Tables.QuestionTable))
                Create.Table<Question>().Do();
        }
    }
}