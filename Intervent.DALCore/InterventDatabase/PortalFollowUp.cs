namespace Intervent.DAL
{
    using System.Collections.Generic;

    public partial class PortalFollowUp
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PortalFollowUp()
        {
            KitsinPortalFollowUps = new HashSet<KitsinPortalFollowUp>();
        }
        public int Id { get; set; }

        public int FollowupTypeId { get; set; }

        public int PortalId { get; set; }

        public bool LabIntegration { get; set; }

        public byte ProgramType { get; set; }

        public virtual FollowUpType FollowUpType { get; set; }

        public virtual Portal Portal { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KitsinPortalFollowUp> KitsinPortalFollowUps { get; set; }
    }
}
