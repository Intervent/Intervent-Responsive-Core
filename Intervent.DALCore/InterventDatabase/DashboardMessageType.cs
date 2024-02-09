namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class DashboardMessageType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DashboardMessageType()
        {
            UserDashboardMessages = new HashSet<UserDashboardMessage>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string MessageTemplate { get; set; }

        [Required]
        [StringLength(100)]
        public string Type { get; set; }

        [StringLength(50)]
        public string? Image { get; set; }

        [StringLength(50)]
        public string? Alt { get; set; }

        [StringLength(100)]
        public string? Url { get; set; }

        [StringLength(10)]
        public string? languageCode { get; set; }

        [Required]
        [StringLength(10)]
        public string LanguageItem { get; set; }

        [Required]
        public byte NotificationType { get; set; }

        [StringLength(50)]
        public string? Subject { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserDashboardMessage> UserDashboardMessages { get; set; }
    }
}
