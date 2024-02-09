namespace Intervent.Web.DTO
{
    public class GetEligAndIntuityEligByPortalResponse
    {

        public IList<IntuityEligibilityDto> IntuityEligibilities { get; set; }

        public IList<EligibilityDto> Eligibilities { get; set; }

    }
}
