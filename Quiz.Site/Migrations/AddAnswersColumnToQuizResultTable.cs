using Quiz.Site.Models;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Quiz.Site.Migrations
{
    /// <summary>
    ///  creates a column in the table if it's not already there
    /// </summary>
    public class AddAnswersColumnToQuizResultTable : MigrationBase
    {
        public AddAnswersColumnToQuizResultTable(IMigrationContext context)
            : base(context)
        {
        }

        protected override void Migrate()
        {
            if (TableExists(Quiz.Tables.QuizResultTable))
            {
                if(!ColumnExists(Quiz.Tables.QuizResultTable, "answers"))
                {
                    Create.Column("answers")
                        .OnTable(Quiz.Tables.QuizResultTable)
                        .AsString()
                        .Nullable()
                        .Do();
                }
            }
        }
    }
}