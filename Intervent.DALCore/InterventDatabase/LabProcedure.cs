namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class LabProcedure
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LabProcedure()
        {
            PortalLabProcedures = new HashSet<PortalLabProcedure>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        [StringLength(50)]
        public string? ProcedureCode { get; set; }

        [StringLength(50)]
        public string? ProcedureAnswer { get; set; }

        [StringLength(50)]
        public string? ProcedureAnswerCode { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PortalLabProcedure> PortalLabProcedures { get; set; }
    }
}
