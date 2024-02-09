namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class TimeZone
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TimeZone()
        {
            Users = new HashSet<User>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string TimeZoneId { get; set; }

        [StringLength(250)]
        public string? TimeZoneDisplay { get; set; }

        [Column("TimeZone")]
        public string? TimeZone1 { get; set; }

        [StringLength(500)]
        public string? CountryFilterList { get; set; }

        [StringLength(100)]
        public string? TimeZoneLocation { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users { get; set; }
    }
}
