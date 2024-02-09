using System.ComponentModel.DataAnnotations;

namespace Intervent.DAL
{
    public partial class FollowUpType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FollowUpType()
        {
            PortalFollowUps = new HashSet<PortalFollowUp>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string? Type { get; set; }

        public int Days { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PortalFollowUp> PortalFollowUps { get; set; }
    }
}
