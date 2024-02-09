namespace Intervent.Web.DTO
{
    public class GetEligibilityResponse
    {
        public EligibilityDto Eligibility { get; set; }
    }

    public class GetIntuityEligibilityResponse
    {
        public EligibilityDto Eligibility { get; set; }

        public int Count { get; set; }

        public bool Status { get; set; }
    }
}
