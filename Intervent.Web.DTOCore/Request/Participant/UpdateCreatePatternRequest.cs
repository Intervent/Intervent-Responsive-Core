namespace Intervent.Web.DTO
{
    public class UpdatePatternCreationRequest
    {
        public int UserId { get; set; }

        public bool Status { get; set; }

        public DateTime PatternCreationDate { get; set; }

        public int systemAdminId { get; set; }

        public IntuityEligibilityLogDto IntuityEligibilityLog { get; set; }
    }
}
