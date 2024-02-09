namespace Intervent.Web.DTO
{
    public class GetPrevYearStatusRequest
    {
        public int userId { get; set; }

        public bool getCoachingInfo { get; set; }

        public bool getFollowupDetails { get; set; }

        public string timeZone { get; set; }
    }
}
