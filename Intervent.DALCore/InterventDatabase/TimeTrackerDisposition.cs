using System.ComponentModel.DataAnnotations;

namespace Intervent.DAL
{
    public partial class TimeTrackerDisposition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TimeTrackerDisposition()
        {
            UserTimeTrackers = new HashSet<UserTimeTracker>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        public string Type { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        public bool ShowInUI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserTimeTracker> UserTimeTrackers { get; set; }
    }
}
