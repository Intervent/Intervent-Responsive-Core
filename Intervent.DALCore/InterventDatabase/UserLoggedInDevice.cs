namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class UserLoggedInDevice
    {
        public UserLoggedInDevice()
        {
            SecurityCodes = new HashSet<SecurityCode>();
            UserLogs = new HashSet<UserLog>();
        }

        public int Id { get; set; }

        [StringLength(250)]
        public string? Token { get; set; }

        public int UserId { get; set; }

        [Required]
        [StringLength(250)]
        public string DeviceId { get; set; }

        [StringLength(250)]
        public string? RefreshToken { get; set; }

        public DateTime? ExpiresOn { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SecurityCode> SecurityCodes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserLog> UserLogs { get; set; }

        public virtual User User { get; set; }
    }
}
