namespace Intervent.Web.DTO
{
    public class GetCoachAvailabilityDetailsResponse
    {
        public List<string> Days { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

    }
}
