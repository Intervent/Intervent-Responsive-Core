namespace Intervent.Web.DTO
{
    public class ListClaimProcessEligibilityResponse
    {
        public IEnumerable<ClaimProcessEligibilityDto> Eligibility { get; set; }
    }

    public class ListClaimProcessCrothalIDResponse
    {
        public IEnumerable<CrothalIdDto> CrothalIds { get; set; }
    }

    public class ListClaimProcessClaimCodeResponse
    {
        public IEnumerable<ClaimCodeDto> ClaimCodes { get; set; }
    }

    public class ListClaimProcessInsuranceSummaryResponse
    {
        public IEnumerable<ClaimProcessInsuranceSummaryDto> InsuranceSummaries { get; set; }
    }

    public class ListClaimProcessEnrolledDataResponse
    {
        public IEnumerable<ClaimProcessEnrolledDataDto> EnrolledDatas { get; set; }
    }

    public class ListClaimProcessHRAResponse
    {
        public IEnumerable<ClaimProcessHRADto> HRAs { get; set; }
    }

    public class ListClaimProcessTherapeuticClassCodeResponse
    {
        public IEnumerable<ClaimProcessTherapeuticClassCodeDto> TherapeuticClassCodes { get; set; }
    }

    public class ListClaimProcessLivongoICDCodeResponse
    {
        public IEnumerable<LivongoICDCodesDto> ICDCodes { get; set; }
    }

    public class ListClaimProcessLivongoNDCCodeResponse
    {
        public IEnumerable<LivongoNDCCodesDto> NDCCodes { get; set; }
    }

    public class ClaimConditionCodeResponse
    {
        public IEnumerable<ClaimConditionCodeDto> ClaimConditionCodes { get; set; }
    }

    public class AddOrEditInsuranceSummaryResponse
    {
        public InsuranceSummaryDto InsuranceSummary { get; set; }
    }
}
