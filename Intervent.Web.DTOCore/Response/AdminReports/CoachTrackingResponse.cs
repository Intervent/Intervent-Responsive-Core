namespace Intervent.Web.DTO
{
    public class CoachTrackingResponse
    {
        public List<CoachTrackingReport_ResultDto> coachTrackingRecords { get; set; }

        public int totalRecords { get; set; }
    }
}
