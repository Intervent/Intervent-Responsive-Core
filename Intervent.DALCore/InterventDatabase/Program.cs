namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Program()
        {
            KitsinPrograms = new HashSet<KitsinProgram>();
            ProgramsinPortals = new HashSet<ProgramsinPortal>();
        }

        public int Id { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        public byte ProgramType { get; set; }

        public byte? RiskLevel { get; set; }

        public bool Smoking { get; set; }

        public bool Active { get; set; }

        [StringLength(100)]
        public string? ImageUrl { get; set; }

        public bool Pregancy { get; set; }

        [StringLength(50)]
        public string? Code { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KitsinProgram> KitsinPrograms { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProgramsinPortal> ProgramsinPortals { get; set; }
    }
}
