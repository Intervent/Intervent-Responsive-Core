namespace Intervent.Web.DTO
{
    public class GetProcessedEligibilityRequest
    {
        public List<string> uniqueIds { get; set; }

        public int portalId { get; set; }
    }
}
