namespace Intervent.Web.DTO
{
    public class ListClaimsConditionsResponse
    {
        public IEnumerable<CandidateConditionsDto> CandidateConditionsCost { get; set; }

        public IEnumerable<CandidateReasonForLastChangeDto> InsuranceSummary { get; set; }
    }
}
