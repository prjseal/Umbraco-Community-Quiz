
using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Quiz.Site.Models
{
    [TableName(Quiz.Tables.QuestionTable)]
    [PrimaryKey("id")]
    [ExplicitColumns]
    public class Question
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("datecreated")]
        public DateTime DateCreated { get; set; }

        [Column("dateupdated")]
        public DateTime DateUpdated { get; set; }

        [Column("authorMemberId")]
        public string AuthorMemberId { get; set; }

        [Column("questionText")]
        public string QuestionText { get; set; }

        [Column("correctAnswer")]
        public string CorrectAnswer { get; set; }

        [Column("wrongAnswer1")]
        public string WrongAnswer1 { get; set; }

        [Column("wrongAnswer2")]
        public string WrongAnswer2 { get; set; }

        [Column("wrongAnswer3")]
        public string WrongAnswer3 { get; set; }

        [Column("moreInfoLink")]
        public string MoreInfoLink { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("correctAnswerPosition")]
        public int CorrectAnswerPosition { get; set; }
    }
}
