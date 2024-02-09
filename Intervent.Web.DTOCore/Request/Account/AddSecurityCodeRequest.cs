namespace Intervent.Web.DTO
{
    public class AddSecurityCodeRequest
    {
        public int userId { get; set; }

        public string code { get; set; }

        public int deviceId { get; set; }

        public byte status { get; set; }
    }
}
