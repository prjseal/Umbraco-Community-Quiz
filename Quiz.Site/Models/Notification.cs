using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Quiz.Site.Models
{
    [TableName(Quiz.Tables.NotificationTable)]
    [PrimaryKey("id")]
    [ExplicitColumns]
    public class Notification
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("badgeId")]
        public string BadgeId { get; set; }

        [Column("datecreated")]
        public DateTime DateCreated { get; set; }

        [Column("memberId")]
        public int MemberId { get; set; }

        [Column("message")]
        public string Message { get; set; }
    }
}
