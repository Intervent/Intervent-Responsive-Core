namespace Intervent.Web.DTO
{
    public class ListEligibilityResponse
    {
        public IList<EligibilityDto> eligibilityList { get; set; }

        public int totalRecords { get; set; }

        public List<ListEligibilityResultDto> Eligibilities { get; set; }
    }
}
