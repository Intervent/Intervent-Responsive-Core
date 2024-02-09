namespace Intervent.Web.DTO
{
    public class UserEligibleToSendCodeResponse
    {
        public bool isEligible { get; set; }

        public DateTime waitTime { get; set; }
    }
}
