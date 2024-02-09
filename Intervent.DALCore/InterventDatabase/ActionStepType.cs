namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class ActionStepType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ActionStepType()
        {
            HRA_ActionSteps = new HashSet<HRA_ActionSteps>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string RiskFactor { get; set; }

        [StringLength(255)]
        public string? Statement { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        public byte Order { get; set; }

        [StringLength(255)]
        public string? StatementIfNull { get; set; }

        [StringLength(255)]
        public string? HelpStatement { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HRA_ActionSteps> HRA_ActionSteps { get; set; }
    }
}
