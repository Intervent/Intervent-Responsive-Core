namespace Intervent.Web.DTO
{
    public class GetEligibilityRequest
    {
        public int? EligibilityId { get; set; }

        public List<int> PortalIds { get; set; }

        public string UniqueId { get; set; }
    }
}
