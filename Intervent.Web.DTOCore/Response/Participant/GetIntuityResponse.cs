namespace Intervent.Web.DTO
{
    public class GetIntuityResponse
    {
        public IntuityEligibilityLogDto IntuityEligibilityLog { get; set; }

        public IntuityEligibilityDto IntuityEligibility { get; set; }

        public IList<IntuityFulfillmentsDto> IntuityFulfillments { get; set; }

        public IList<IntuityFulfillmentRequestsDto> IntuityFulfillmentRequests { get; set; }

        public IList<IntuityQOHDto> IntuityQOH { get; set; }

        public bool OverrideStatus { get; set; }

        public bool ShowNewForm { get; set; }

        public bool UserEligible { get; set; }

        public bool ShowFulfillment { get; set; }

        public int IntuityEligibilityId { get; set; }

        public DateTime? PatternsRegDate { get; set; }

        public DateTime? FormSubmittedDate { get; set; }
    }
}
