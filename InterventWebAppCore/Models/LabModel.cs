using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class AddLabModel
    {
        public LabDto Lab { get; set; }

        public string BloodTestDate { get; set; }

        public float? HeightFt { get; set; }

        public float? HeightInch { get; set; }

        public IList<MeasurementsDto> Measurements { get; set; }

        public bool SaveNew { get; set; }

        public int? HRAValidity { get; set; }

        public int Id { get; set; }

        public bool updateLab { get; set; }

        public bool fromEligibility { get; set; }

        public int? updatedBy { get; set; }

        public int participantId { get; set; }

        public int participantPortalId { get; set; }

        public int? userinProgramId { get; set; }

        public int? integrationWith { get; set; }

        public int? Unit { get; set; }

        public string DateFormat { get; set; }

        public bool IsParticipantView { get; set; }
    }

    public class LabResults
    {
        public LabDto incompleteLab { get; set; }

        public IList<LabReferenceRangeDto> LabReferences { get; set; }

        public IList<LabDto> Labs { get; set; }

        public int? incompleteLabId { get; set; }

        public int? SwitchCount { get; set; }

        public IList<MeasurementsDto> Measurements { get; set; }

        public int LabsForCurrentPortal { get; set; }

        public bool LabIntegration { get; set; }

        public string UserName { get; set; }

        public string ValidLabs { get; set; }

        public bool hasRecentLabs { get; set; }

        public int? DiagnosticLabId { get; set; }

        public IEnumerable<SelectListItem> LabRejectionReasons { get; set; }

        public IEnumerable<SelectListItem> LabList { get; set; }

        public int OrganizationId { get; set; }

        public bool IsParticipantView { get; set; }

        public int? IntegrationWith { get; set; }

        public int? Gender { get; set; }

        public int? Unit { get; set; }

        public string DateFormat { get; set; }

        public int southUniversityOrgId { get; set; }

        public int? ParticipantPortalId { get; set; }

        public bool HasActivePortal { get; set; }
    }

    public class ReportListModel
    {
        public int page { get; set; }

        public int pageSize { get; set; }

        public int? totalRecords { get; set; }

    }

    [Serializable]
    public class FilterReportTempData
    {
        public int page { get; set; }

        public int pageSize { get; set; }

        public int totalRecords { get; set; }

        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }

        public int? userId { get; set; }

        public int alertType { get; set; }

        public int labsource { get; set; }

        public int? organization { get; set; }

        public int?[] organizations { get; set; }

        public int?[] organizationIndex { get; set; }

        public int? appointmentType { get; set; }

        public string TimeZoneType { get; set; }

        public string isReviewed { get; set; }

        public string language { get; set; }

        public int? coach { get; set; }

        public bool forDownload { get; set; }

        public int? Status { get; set; }

        public int[] Task { get; set; }

        public int[] TaskIndex { get; set; }

        public int? Owner { get; set; }

        public string TaskStatus { get; set; }

        public int? portalId { get; set; }

        public int? kit { get; set; }

        public int? filterType { get; set; }

        public int? contactId { get; set; }

        public int? assessmentType { get; set; }

        public string userStatus { get; set; }

    }

}