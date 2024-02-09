namespace Intervent.Web.DTO
{
    public class VerifyUserTokenRequest
    {
        public int userId { get; set; }

        public string purpose { get; set; }

        public string token { get; set; }
    }
}
