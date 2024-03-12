namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CRM_Dispositions")]
    public partial class CRM_Disposition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CRM_Disposition()
        {
            CRM_Notes = new HashSet<CRM_Note>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Disposition { get; set; }

        [Column("Complaint ")]
        public bool Complaint { get; set; }

        public int? CategoryId { get; set; }

        public byte? Type { get; set; }

        public bool EligibleforActivity { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_Note> CRM_Notes { get; set; }
    }
}
