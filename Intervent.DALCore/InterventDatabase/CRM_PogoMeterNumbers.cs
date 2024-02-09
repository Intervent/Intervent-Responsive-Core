namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class CRM_PogoMeterNumbers
    {
        public CRM_PogoMeterNumbers()
        {
            CRM_Notes = new HashSet<CRM_Note>();
        }
        public int Id { get; set; }

        public int CRMContactId { get; set; }

        [Required]
        [StringLength(50)]
        public string PogoMeterNumber { get; set; }

        public bool IsActive { get; set; }


        public virtual CRM_Contact CRM_Contacts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_Note> CRM_Notes { get; set; }
    }
}
