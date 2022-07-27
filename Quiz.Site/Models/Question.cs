
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

        [Column("createdby")]
        public string CreatedBy { get; set; }

        [Column("lasteditedby")]
        public string LastEditedBy { get; set; }

        [Column("text")]
        public string Text { get; set; }

        [Column("possibleanswers")]
        public string PossibleAnswers { get; set; }

        [Column("correctanswers")]
        public string CorrectAnswers { get; set; }

        [Column("links")]
        public string Links { get; set; }

        [Column("tags")]
        public string Tags { get; set; }

        [Column("approved")]
        public bool Approved { get; set; }
    }
}
