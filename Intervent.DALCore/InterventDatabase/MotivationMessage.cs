namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class MotivationMessage
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MotivationMessage()
        {
            AssignedMotivationMessages = new HashSet<AssignedMotivationMessage>();
        }

        public int Id { get; set; }

        [StringLength(100)]
        public string? Subject { get; set; }

        public string? MessageContent { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssignedMotivationMessage> AssignedMotivationMessages { get; set; }
    }
}
