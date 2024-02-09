namespace Intervent.Web.DTO
{
    public class ExternalUserDto
    {
        public int UserId { get; set; }

        public int ExternalDevice { get; set; }

        public string ExternalId { get; set; }

        public string DeviceSyncURL { get; set; }

        public string ExternalToken { get; set; }

        public string MobileToken { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public int? DeviceCount { get; set; }

        public bool IsActive { get; set; }

        public virtual UserDto User { get; set; }
    }
}
