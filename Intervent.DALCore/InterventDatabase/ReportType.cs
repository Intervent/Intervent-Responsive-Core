namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class ReportType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ReportType()
        {
            FaxedReports = new HashSet<FaxedReport>();
        }
        public int Id { get; set; }

        [StringLength(50)]
        public string? Type { get; set; }

        [StringLength(50)]
        public string? ReportName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FaxedReport> FaxedReports { get; set; }
    }
}
