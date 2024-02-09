namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("NotificationMessage")]
    public partial class NotificationMessage
    {
        public long Id { get; set; }

        public long NotificationEventID { get; set; }

        [Required]
        [StringLength(255)]
        public string FromAddress { get; set; }

        [Required]
        [StringLength(500)]
        public string ToAddress { get; set; }

        [StringLength(500)]
        public string? SubjectLine { get; set; }

        [Required]
        public string MessageBody { get; set; }

        [StringLength(900)]
        public string? BccAddress { get; set; }

        [StringLength(900)]
        public string? CcAddress { get; set; }

        [StringLength(255)]
        public string? Attachment { get; set; }

        public DateTime Date { get; set; }

        public virtual NotificationEvent NotificationEvent { get; set; }
    }
}
