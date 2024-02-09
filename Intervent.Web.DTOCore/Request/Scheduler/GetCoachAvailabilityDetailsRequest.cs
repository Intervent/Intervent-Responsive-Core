namespace Intervent.Web.DTO
{
    public class GetCoachAvailabilityDetailsRequest
    {
        public long RefId { get; set; }

        public DateTime? StartDate { get; set; }

        public string TimeZone { get; set; }
    }
}
