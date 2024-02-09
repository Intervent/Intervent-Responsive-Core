namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Messages")]
    public partial class Message
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Message()
        {
            MessageRecipients = new HashSet<MessageRecipient>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Subject { get; set; }

        public int CreatorId { get; set; }

        [Required]
        public string MessageBody { get; set; }

        [Column(TypeName = "date")]
        public DateTime CreateDate { get; set; }

        public int? ParentMessageId { get; set; }

        [StringLength(250)]
        public string? Attachment { get; set; }

        public bool IsSent { get; set; }

        public bool? NoActionNeeded { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CaseClosedDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MessageRecipient> MessageRecipients { get; set; }

        public virtual User User { get; set; }
    }
}
