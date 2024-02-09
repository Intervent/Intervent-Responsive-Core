namespace Intervent.Web.DTO
{
    public sealed class AddEditIntuityEligibilityRequest
    {
        public IntuityEligibilityDto IntuityEligibility { get; set; }

        public bool UpdateStatus { get; set; }

        public int UserId { get; set; }
    }
}
