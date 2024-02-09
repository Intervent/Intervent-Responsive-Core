using Intervent.Web.DTO;

namespace Intervent.Business.Claims
{
    public interface IClaimManager
    {
        ListClaimsConditionsResponse GetConditionsList(GetCandidateConditionListRequest request);
        ListClaimsMedicationResponse GetMedicationsList(GetCandidateMedicationListRequest request);
        IEnumerable<DiagnosisCodeDto> GetDiagnosisCodes();

        void ProcessClaims();
    }
}
