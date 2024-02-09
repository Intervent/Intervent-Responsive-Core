namespace Intervent.Web.DTO
{
    public sealed class AddEditEligibilityResponse
    {
        public EligibilityDto Eligibility { get; set; }

        public bool success { get; set; }
    }

    public sealed class BulkAddEditEligibilityResponse
    {
        public bool success { get; set; }
    }

    public sealed class BulkEligibilityUpdateResponse
    {
        public bool success { get; set; }
    }
}
