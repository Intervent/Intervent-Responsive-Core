namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class UserWearableDevice
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string ExternalUserId { get; set; }

        public int WearableDeviceId { get; set; }

        public bool IsActive { get; set; }

        public string? Token { get; set; }

        public string? RefreshToken { get; set; }

        public string? OauthTokenSecret { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? OffsetFromUTC { get; set; }

        public string? DeviceId { get; set; }

        public string? Scope { get; set; }

        public virtual User User { get; set; }

        public virtual WearableDevice WearableDevice { get; set; }
    }
}
