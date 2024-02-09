namespace Intervent.Web.DTO
{
    public class UserLogsDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime LastAccessedOn { get; set; }

        public DateTime? LoggedOutTime { get; set; }

        public bool? IsSuccess { get; set; }

        public int? LoggedInDeviceId { get; set; }

        public virtual UserLoggedInDevicesDto UserLoggedInDevice { get; set; }

        public virtual UserDto User { get; set; }
    }
}
