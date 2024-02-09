namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ApptCallTemplate")]
    public partial class ApptCallTemplate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ApptCallTemplate()
        {
            ApptCallIntervals = new HashSet<ApptCallInterval>();
            ProgramsinPortals = new HashSet<ProgramsinPortal>();
        }

        public int Id { get; set; }

        public int NoOfCalls { get; set; }

        [Required]
        [StringLength(100)]
        public string TemplateName { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int UpdatedBy { get; set; }

        public bool IsActive { get; set; }

        public int? NoOfWeeks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ApptCallInterval> ApptCallIntervals { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProgramsinPortal> ProgramsinPortals { get; set; }
    }
}
