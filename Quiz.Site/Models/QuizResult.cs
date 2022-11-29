
using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Quiz.Site.Models
{
    [TableName(Quiz.Tables.QuizResultTable)]
    [PrimaryKey("id")]
    [ExplicitColumns]
    public class QuizResult
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("quizId")]
        public string QuizId { get; set; }

        [Column("datecreated")]
        public DateTime DateCreated { get; set; }

        [Column("memberId")]
        public int MemberId { get; set; }

        [Column("score")]
        public int Score { get; set; }

        [Column("total")]
        public int Total { get; set; }

        [Column("answers")]
        public string Answers { get; set; }
    }
}
