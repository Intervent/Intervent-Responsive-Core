namespace Intervent.Web.DTO
{
    public class UserWearableDeviceDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string ExternalUserId { get; set; }

        public int WearableDeviceId { get; set; }

        public bool IsActive { get; set; }

        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public string OauthTokenSecret { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? UpdateOn { get; set; }

        public int? OffsetFromUTC { get; set; }

        public string DeviceId { get; set; }

        public string Scope { get; set; }

        public virtual UserDto User { get; set; }

        public virtual WearableDeviceDto WearableDevice { get; set; }
    }
}
