namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("NotificationTemplate")]
    public partial class NotificationTemplate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NotificationTemplate()
        {
            NotificationEvents = new HashSet<NotificationEvent>();
            NotificationTemplateTranslations = new HashSet<NotificationTemplateTranslation>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Description { get; set; }

        public int NotificationEventTypeId { get; set; }

        public bool Active { get; set; }

        public int TemplateRendererId { get; set; }

        public bool DataPacketRequired { get; set; }

        [Required]
        [StringLength(250)]
        public string EmailFrom { get; set; }

        [Required]
        [StringLength(250)]
        public string EmailSubject { get; set; }

        public int NotificationContactMethodId { get; set; }

        [Required]
        [StringLength(100)]
        public string LastUpdatedUser { get; set; }

        public DateTime LastUpdatedDate { get; set; }

        public string? TemplateSource { get; set; }

        public bool? HasAttachment { get; set; }

        public virtual NotificationContactMethod NotificationContactMethod { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificationEvent> NotificationEvents { get; set; }

        public virtual NotificationEventType NotificationEventType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificationTemplateTranslation> NotificationTemplateTranslations { get; set; }

    }
}
