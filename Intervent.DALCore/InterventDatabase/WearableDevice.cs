namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class WearableDevice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WearableDevice()
        {
            UserWearableDevices = new HashSet<UserWearableDevice>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Icon { get; set; }

        [Required]
        [StringLength(50)]
        public string AuthUrl { get; set; }

        public bool IsActive { get; set; }

        public byte? Type { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserWearableDevice> UserWearableDevices { get; set; }
    }
}
