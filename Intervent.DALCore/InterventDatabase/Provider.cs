using System.ComponentModel.DataAnnotations;

namespace Intervent.DAL
{

    public partial class Provider
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Provider()
        {
            UserDoctorInfoes = new HashSet<UserDoctorInfo>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        public int? State { get; set; }

        public int? Country { get; set; }

        [StringLength(15)]
        public string? Zip { get; set; }

        [StringLength(50)]
        public string? FaxNumber { get; set; }

        [StringLength(50)]
        public string? PhoneNumber { get; set; }

        public bool Active { get; set; }

        public int? OrgId { get; set; }

        public virtual Country Country1 { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual State State1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserDoctorInfo> UserDoctorInfoes { get; set; }
    }
}
