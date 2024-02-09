namespace Intervent.Web.DTO
{
    public class UserEligibleToSendCodeRequest
    {
        public int userId { get; set; }

        public int loggedinDeviceId { get; set; }

        public byte active { get; set; }

        public byte expired { get; set; }
    }
}