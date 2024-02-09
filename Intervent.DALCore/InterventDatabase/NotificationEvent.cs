namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("NotificationEvent")]
    public partial class NotificationEvent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NotificationEvent()
        {
            NotificationMessages = new HashSet<NotificationMessage>();
        }

        public long Id { get; set; }

        public int NotificationEventTypeId { get; set; }

        public int NotificationTemplateId { get; set; }

        public int NotificationStatusId { get; set; }

        [StringLength(4000)]
        public string? DataPacket { get; set; }

        public int? UserId { get; set; }

        public DateTime NotificationEventDate { get; set; }

        [StringLength(250)]
        public string? Subject { get; set; }

        [StringLength(500)]
        public string? ToEmailAddress { get; set; }

        [StringLength(100)]
        public string? FromEmailAddress { get; set; }

        [StringLength(500)]
        public string? CcAddress { get; set; }

        [StringLength(500)]
        public string? BccAddress { get; set; }

        [StringLength(255)]
        public string? Attachment { get; set; }

        public int? PortalId { get; set; }

        public string? UniqueId { get; set; }

        public virtual NotificationEventType NotificationEventType { get; set; }

        public virtual NotificationStatu NotificationStatu { get; set; }

        public virtual NotificationTemplate NotificationTemplate { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificationMessage> NotificationMessages { get; set; }
    }
}
