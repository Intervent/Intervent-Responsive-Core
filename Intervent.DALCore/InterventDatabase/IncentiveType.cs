namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class IncentiveType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IncentiveType()
        {
            PortalIncentives = new HashSet<PortalIncentive>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(40)]
        public string Type { get; set; }

        [StringLength(256)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        [StringLength(50)]
        public string? Url { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PortalIncentive> PortalIncentives { get; set; }
    }
}
