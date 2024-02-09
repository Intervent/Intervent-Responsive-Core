namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class CoachingConditions
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Condition { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PortalCoachingConditions> PortalCoachingConditions { get; set; }
    }
}
