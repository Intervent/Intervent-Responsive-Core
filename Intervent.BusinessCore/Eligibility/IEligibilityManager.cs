using Intervent.Web.DTO;

namespace Intervent.Business.Eligibility
{
    public interface IEligibilityManager
    {
        IEnumerable<OrganizationDto> GetOrganizationsEligibleForImport();

        void LoadEligibilityFiles();

        AddEditEligibilityResponse AddEditEligibilityRecord(AddEditEligibilityRequest request);
        void AddEligibilityImportLog(string action, string errorDetails, int portalId, string uniqueId, bool isLoadError, int? eligibilityId, string firstName = null, string lastName = null, string changes = null, string createdByUser = null);

        void UpdateUserEligibilitySetting(UpdateUserEligibilitySettingRequest request);
        GetUserEligibilitySettingResponse GetUserEligibilitySetting(GetUserEligibilitySettingRequest request);

        int TerminateNotSentRecords(int portalId);

        int DeleteDuplicateRecords(int portalId);

    }
}
