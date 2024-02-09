namespace Intervent.Web.DTO
{
    public class DeleteCoachAvailabilityResponse
    {
        public bool Status { get; set; }

        public List<String> bookedAvailabilities { get; set; }
    }
}
