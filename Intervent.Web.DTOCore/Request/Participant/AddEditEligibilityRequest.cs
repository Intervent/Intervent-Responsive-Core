namespace Intervent.Web.DTO
{
    public sealed class AddEditEligibilityRequest
    {
        public EligibilityDto Eligibility { get; set; }

    }

    public sealed class BulkAddEditEligibilityRequest
    {
        public IEnumerable<EligibilityDto> Eligibilities { get; set; }

    }

}
