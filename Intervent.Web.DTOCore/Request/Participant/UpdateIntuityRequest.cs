namespace Intervent.Web.DTO
{
    public class UpdateIntuityRequest
    {
        public IntuityEligibilityLogDto IntuityEligibilityLog { get; set; }

        public int PortalId { get; set; }

        public int EligibilityId { get; set; }

        public bool UpdateProfile { get; set; }

        public int UserId { get; set; }
    }
}
