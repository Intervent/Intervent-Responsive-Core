using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intervent.DAL
{
    [Table("ContactRequirementsAlert")]
    public partial class ContactRequirementsAlert
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ContactRequirementsAlert()
        {
            ContactRequirements = new HashSet<ContactRequirement>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string AlertType { get; set; }

        [Required]
        [StringLength(200)]
        public string? CoachingMessage { get; set; }

        [Required]
        [StringLength(200)]
        public string? SelfHelpMessage { get; set; }

        public int? CoachingOutBound { get; set; } //Days count

        public int? SelfHelpOutBound { get; set; } //Days count

        public int? Min { get; set; }

        public int? Max { get; set; }

        [StringLength(25)]
        public string? Code { get; set; }

        public int? Frequency { get; set; } //Days count

        public int? Level { get; set; }

        public int? AppointmentWithIn { get; set; } //Days count

        public int? DoNotTrack { get; set; } //Days count

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContactRequirement> ContactRequirements { get; set; }
    }
}
