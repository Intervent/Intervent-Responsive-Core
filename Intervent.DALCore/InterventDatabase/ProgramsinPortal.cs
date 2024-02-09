namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ProgramsinPortal
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProgramsinPortal()
        {
            UsersinPrograms = new HashSet<UsersinProgram>();
        }

        public int Id { get; set; }

        public int PortalId { get; set; }

        public int ProgramId { get; set; }

        [Column(TypeName = "money")]
        public decimal? Cost { get; set; }

        public string? MoreInfo { get; set; }

        public bool Active { get; set; }

        [StringLength(100)]
        public string? NameforUser { get; set; }

        [StringLength(512)]
        public string? DescriptionforUser { get; set; }

        [StringLength(100)]
        public string? NameforAdmin { get; set; }

        [StringLength(512)]
        public string? DescriptionforAdmin { get; set; }

        public string? NameforUserLanguageItem { get; set; }

        public string? DescforUserLanguageItem { get; set; }

        public int? ApptCallTemplateId { get; set; }

        public virtual ApptCallTemplate ApptCallTemplate { get; set; }

        public virtual Portal Portal { get; set; }

        public virtual Program Program { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsersinProgram> UsersinPrograms { get; set; }

        public int? SortOrder { get; set; }
    }
}
