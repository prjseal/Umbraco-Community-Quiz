
using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Quiz.Site.Models
{
    [TableName(Quiz.Tables.ReadNotificationTable)]
    [PrimaryKey("id")]
    [ExplicitColumns]
    public class ReadNotification
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("notificationId")]
        public string NotificationId { get; set; }

        [Column("datecreated")]
        public DateTime DateCreated { get; set; }
    }
}
