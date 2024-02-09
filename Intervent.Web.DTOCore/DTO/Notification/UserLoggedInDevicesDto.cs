namespace Intervent.Web.DTO
{
    public class UserLoggedInDevicesDto
    {
        public UserLoggedInDevicesDto()
        {
            UserLogs = new HashSet<UserLogsDto>();
        }

        public int Id { get; set; }

        public string Token { get; set; }

        public int UserId { get; set; }

        public string DeviceId { get; set; }

        public virtual ICollection<UserLogsDto> UserLogs { get; set; }

        public virtual UserDto User { get; set; }
    }
}