using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class WellnessDataModel
    {
        public IList<MeasurementsDto> Measurements { get; set; }

        public WellnessDataDto wellnessData { get; set; }

        public IEnumerable<SelectListItem> ExerIntList { get; set; }

        public IEnumerable<SelectListItem> ContractStatusList { get; set; }

        public IEnumerable<SelectListItem> Ratings { get; set; }

        public IEnumerable<SelectListItem> DiabetesTypeList { get; set; }

        public IEnumerable<SelectListItem> GlucoseCheckCountList { get; set; }

        public IEnumerable<SelectListItem> ClaudicationScale { get; set; }

        public bool tobaccoUser { get; set; }

        public bool showDiabetes { get; set; }

        public string WeightText { get; set; }

        public string WaistText { get; set; }

        public int? Age { get; set; }

        public bool updatedbyUser { get; set; }

        public int userId { get; set; }

        public int? integrationWith { get; set; }

        public int? gender { get; set; }

        public string dateFormat { get; set; }

        public bool allowCardiacQuestion { get; set; }

        public bool hasActivePortal { get; set; }

        public bool GumBool
        {
            get { return wellnessData != null && wellnessData.Gum > 0; }
            set { wellnessData.Gum = value ? Convert.ToByte(1) : Convert.ToByte(0); }
        }

        public bool PatchBool
        {
            get { return wellnessData != null && wellnessData.Patch > 0; }
            set { wellnessData.Patch = value ? Convert.ToByte(1) : Convert.ToByte(0); }
        }

        public bool LozengeBool
        {
            get { return wellnessData != null && wellnessData.Lozenge > 0; }
            set { wellnessData.Lozenge = value ? Convert.ToByte(1) : Convert.ToByte(0); }
        }

        public bool InhalerBool
        {
            get { return wellnessData != null && wellnessData.Inhaler > 0; }
            set { wellnessData.Inhaler = value ? Convert.ToByte(1) : Convert.ToByte(0); }
        }

        public bool SprayBool
        {
            get { return wellnessData != null && wellnessData.Spray > 0; }
            set { wellnessData.Spray = value ? Convert.ToByte(1) : Convert.ToByte(0); }
        }

        public bool SteroidBool
        {
            get { return wellnessData != null && wellnessData.Steroid == 1; }
            set { wellnessData.Steroid = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool PancreatitisBool
        {
            get { return wellnessData != null && wellnessData.Pancreatitis == 1; }
            set { wellnessData.Pancreatitis = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool CancerTreatedBool
        {
            get { return wellnessData != null && wellnessData.CancerTreated == 1; }
            set { wellnessData.CancerTreated = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool HemoglobinopathyBool
        {
            get { return wellnessData != null && wellnessData.Hemoglobinopathy == 1; }
            set { wellnessData.Hemoglobinopathy = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool StudyDrugBool
        {
            get { return wellnessData != null && wellnessData.StudyDrug == 1; }
            set { wellnessData.StudyDrug = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }
        public bool GlucoseLoweringBool
        {
            get { return wellnessData != null && wellnessData.GlucoseLowering == 1; }
            set { wellnessData.GlucoseLowering = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool WeightLossBool
        {
            get { return wellnessData != null && wellnessData.WeightLoss == 1; }
            set { wellnessData.WeightLoss = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool sameDeviceBool
        {
            get { return wellnessData != null && wellnessData.sameDevice == 1; }
            set { wellnessData.sameDevice = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }
    }

    public class NotesModel
    {
        public NotesDto Note { get; set; }

        public IEnumerable<SelectListItem> NoteTypes { get; set; }

        public IEnumerable<SelectListItem> ReferralTypes { get; set; }

        public UserTrackingStatusDto UserTrackingStatus { get; set; }

        public bool DisableFields { get; set; }

        public IList<AppointmentDTO> Appointments { get; set; }

        public int? AppId { get; set; }

        public bool isEnrolled { get; set; }

        public string dateFormat { get; set; }

        public int? userinProgramId { get; set; }

        public int? programType { get; set; }

        public int? participantPortalId { get; set; }

        public string enrolledinCoaching { get; set; }

        public bool hasActivePortal { get; set; }
    }

    public class SelectProgramModel
    {
        public string conditions { get; set; }

        public float? CACScore { get; set; }

        public string ReportText { get; set; }

        public string ProgramText { get; set; }

        public string ContactText { get; set; }

        public string FutureAppointmentDate { get; set; }

        public bool hasCoachingConditions { get; set; }

        public string participantName { get; set; }

        public string assessmentName { get; set; }

        public string userStatus { get; set; }

        public int? userinProgramId { get; set; }

        public string nextApptDate { get; set; }

        public bool coachingProgram { get; set; }

        public bool selfHelpProgram { get; set; }

        public bool showProgramOption { get; set; }

        public int? programType { get; set; }

        public string orgContactEmail { get; set; }

        public int participantId { get; set; }

        public int organizationId { get; set; }

    }

    public class ProgramOptionsModel
    {
        public bool hasSelfHelp { get; set; }

        public bool hasCoaching { get; set; }

        public bool showCoaching { get; set; }

        public bool showSelfHelp { get; set; }

        public string coachText { get; set; }

        public bool hasCoachingConditions { get; set; }

        public bool selfScheduling { get; set; }

        public int? programType { get; set; }
    }

    public class ParticipantDashboardModel
    {
        public DateTime? hraCompleteDate { get; set; }

        public int hraPercent { get; set; }

        public bool? profileComplete { get; set; }

        public string ParticipantFirstName { get; set; }

        public bool PreviousPortalAvailable { get; set; }

        public string portalStartDate { get; set; }

        public bool openProfile { get; set; }

        public int? externalReportId { get; set; }

        public bool visitedTab { get; set; }
        public bool hasActivePortal { get; set; }
        public string AssessmentName { get; set; }
        public int? UserinProgramId { get; set; }
        public int? AdminId { get; set; }
        public int UserId { get; set; }
        public int ParticipantId { get; set; }
        public string ParticipantEmail { get; set; }
        public bool MailScoreCard { get; set; }
        public bool ShowSelfScheduling { get; set; }
        public bool IsRescheduling { get; set; }
    }

    public class InitialDashboardModel
    {
        public string dashboardContent { get; set; }
    }

    public class ParticipantSearchModel
    {
        public IEnumerable<SelectListItem> OrganizationList { get; set; }

        public string Organization { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string RiskCode { get; set; }

        public string DateFormat { get; set; }
    }

    public class ParticipantInfoModel
    {
        public UserDto User { get; set; }

        public ParticipantProfile_ResultDto participant { get; set; }

        public IList<AppointmentDTO> Appointments { get; set; }

        public AgeInfo ageInfo { get; set; }

        public int age { get; set; }

        public NotesDto Note { get; set; }

        public string WellnessVision { get; set; }

        public int hraId { get; set; }

        public float? Weight { get; set; }

        public double? BMI { get; set; }

        public string WeightText { get; set; }

        public string WaistText { get; set; }

        public string CoachName { get; set; }

        public WellnessScoreInfo wellnessScoreInfo { get; set; }

        public string hraRisk { get; set; }

        public RisksModel risks { get; set; }

        public string WellnessDate { get; set; }

        public string PregnancyStatus { get; set; }

        public int? ASCVDRisk { get; set; }

        public int? DiabetesRisk { get; set; }

        public string DiabetesRiskText { get; set; }

        public bool labAlert { get; set; }

        public bool hraLink { get; set; }

        public bool challengesLink { get; set; }

        public bool sendReportsLink { get; set; }

        public string claimsReason { get; set; }

        public string claimsDate { get; set; }

        public bool hasEMR { get; set; }

        public UserFormDto patientReleaseForm { set; get; }

        public UserFormDto medicalClearanceForm { set; get; }

        public bool hasExternalReport { set; get; }

        public bool? fromLabReport { get; set; }

        public bool? fromNoShowReport { get; set; }

        public bool? fromCoachReport { get; set; }

        public bool? fromAdminTasks { get; set; }

        public bool? fromCRMProfile { get; set; }

        public bool? fromUnapprovedCarePlan { get; set; }

        public bool? openAppointment { get; set; }

        public string MedicalEligibility { get; set; }

        public string BusinessUnit { get; set; }

        public string URL { get; set; }

        public string TobaccoFlag { get; set; }

        public string ProgramName { get; set; }

        public int? AppId { get; set; }

        public int? CanriskScore { get; set; }

        public float? CanriskA1c { get; set; }

        public float? CanriskGlucose { get; set; }

        public string GlucoseMeasurement { get; set; }

        public string CanriskText { get; set; }

        public string CanriskLabType { get; set; }

        public bool? CompIntroKitsOnTime { get; set; }

        public string SubscriptionPlan { get; set; }

        public string LastAccessedOn { get; set; }

        public int MessageCount { get; set; }

        public int? Gender { get; set; }

        public int? AdminId { get; set; }

        public int ParticipantId { get; set; }

        public int? UserinProgramId { get; set; }

        public int? ProgramType { get; set; }

        public bool? HasActivePortal { get; set; }

        public string DateFormat { get; set; }

        public DateTime? CanriskCompletedOn { get; set; }

        public bool ShowTimeTracker { get; set; }

        public bool IsProfileCompleted { get; set; }

        public bool followUpDue { get; set; }

        public bool dueForRenewal { get; set; }

        public int participantPortalId { get; set; }

        public string enrolledinCoaching { get; set; }

        public int? integrationWith { get; set; }

        public IEnumerable<SelectListItem> TimeTrackingDispositionList { get; set; }

        public string BaseUrl { get; set; }
    }

    public class TimeTrackerModel
    {
        public string UserName { get; set; }

        public DateTime TrackerStartTime { get; set; }

        public bool ShowTimeTracker { get; set; }

        public bool IsParticipantInfoPage { get; set; }

        public int reminderTime { get; set; }

        public IEnumerable<SelectListItem> TimeTrackingDispositionList { get; set; }
    }

    public class CheckRiskModel
    {
        public int? Gender { get; set; }

        public int Age { get; set; }

        public float? HDL { get; set; }

        public string HDLText { get; set; }

        public float HDLMin { get; set; }

        public float HDLMax { get; set; }

        public float ASCVDHDLMin { get; set; }

        public float ASCVDHDLMax { get; set; }

        public float ASCVDSBPMin { get; set; }

        public float ASCVDSBPMax { get; set; }

        public bool ASCVD { get; set; }

        public int Diabetes { get; set; }

        public short? SBP { get; set; }

        public short? DBP { get; set; }

        public float? TotalChol { get; set; }

        public string CholesterolText { get; set; }

        public float TotalCholMin { get; set; }

        public float TotalCholMax { get; set; }

        public float ASCVDCholMin { get; set; }

        public float ASCVDCholMax { get; set; }

        public float? ConversionValue { get; set; }

        public byte? SmokeCig { get; set; }

        public byte? HighBPMed { get; set; }

        public IEnumerable<SelectListItem> RaceList { get; set; }

        public int? Race { get; set; }
    }

    public class MyCoachModel
    {
        public IEnumerable<SelectListItem> Specializations { get; set; }

        public IEnumerable<SelectListItem> Languages { get; set; }

        public IEnumerable<SelectListItem> Timezones { get; set; }

        public int coachId { get; set; }

        public int? apptId { get; set; }

        public string participantTimeZone { get; set; }

        public int? integrationWith { get; set; }

        public string BaseUrl { get; set; }
    }

    public class RescheduleModel
    {
        public UserDto coach { get; set; }

        public string Languages { get; set; }

        public string Speciality { get; set; }

        public string BaseUrl { get; set; }
    }

    public class AssessmentModel
    {
        public bool IsEligibilForHRA { get; set; }

        public bool IsFUAssigned { get; set; }

        public string currentHRAStatus { get; set; }

        public string currentFUStatus { get; set; }

        public Dictionary<string, string> pastAssessments { get; set; }

        public Dictionary<string, string> FUAssessments { get; set; }

        public int? hraId { get; set; }

        public int followUpId { get; set; }

    }

    public class AWVModel
    {
        public IList<AWVDto> AWV { get; set; }
    }

    public class SearchUserModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? Organization { get; set; }

        public int? Id { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string RiskCode { get; set; }

        public int page { get; set; }

        public int pageSize { get; set; }

        public int? totalRecords { get; set; }

        public DateTime? HraStartDate { get; set; }

        public DateTime? HraEndDate { get; set; }
    }

    public class GlucometerData
    {
        public string Name { get; set; }

        public int Value { get; set; }

        public float ChecksperDay { get; set; }

        public int Min { get; set; }

        public int Max { get; set; }

        public int Range1Value { get; set; }

        public int Range1Count { get; set; }

        public int Range2Value { get; set; }

        public int Range2Count { get; set; }

        public int Range3Value { get; set; }

        public int Range3Count { get; set; }

        public int Range4Value { get; set; }

        public int Range4Count { get; set; }

        public int Count { get; set; }

        public int GoalsCount { get; set; }

        public string Notes { get; set; }

        public string Diabetic { get; set; }

        public string dateFrom { get; set; }
        public string dateTo { get; set; }

        public string NameLI { get; set; }

        public int preMealGoalFrom { get; set; }

        public int preMealGoalTo { get; set; }

        public int postMealGoalFrom { get; set; }

        public int postMealGoalTo { get; set; }
    }

    public class NotificationMessageModel
    {
        public IList<NotificationMessageDto> NotificationMessage { get; set; }
    }

    public class HomeModel
    {
        public bool IsModal { get; set; }

        public bool CloseModal { get; set; }

        public bool MobileView { get; set; }
    }

    public class FormModel
    {
        public List<UserFormDto> userForms { get; set; }
    }

    public class CoachModel
    {
        public bool noteCheck { get; set; }

        public bool coachCheck { get; set; }

        public bool bioCheck { get; set; }

    }

    public class CanriskModel
    {
        public CanriskQuestionnaireDto canrisk { get; set; }

        public int? UserId { get; set; }

        public string DateFormat { get; set; }

        public bool HasAdminRole { get; set; }

        public bool DiabetesFatherBool
        {
            get { return canrisk != null && canrisk.DiabetesFather == 1; }
            set { canrisk.DiabetesFather = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool DiabetesMotherBool
        {
            get { return canrisk != null && canrisk.DiabetesMother == 1; }
            set { canrisk.DiabetesMother = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool DiabetesSiblingsBool
        {
            get { return canrisk != null && canrisk.DiabetesSiblings == 1; }
            set { canrisk.DiabetesSiblings = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool DiabetesChildrenBool
        {
            get { return canrisk != null && canrisk.DiabetesChildren == 1; }
            set { canrisk.DiabetesChildren = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool DiabetesOthersBool
        {
            get { return canrisk != null && canrisk.DiabetesOthers == 1; }
            set { canrisk.DiabetesOthers = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool DiabetesNotSureBool
        {
            get { return canrisk != null && canrisk.DiabetesNotSure == 1; }
            set { canrisk.DiabetesNotSure = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool WeightUnit { get; set; }

        public bool HeightUnit { get; set; }

        public bool WaistUnit { get; set; }

        public int year { get; set; }

        public IEnumerable<SelectListItem> years { get; set; }

        public int month { get; set; }

        public IEnumerable<SelectListItem> months { get; set; }

        public int day { get; set; }

        public IEnumerable<SelectListItem> days { get; set; }

        public IEnumerable<SelectListItem> GenderList { get; set; }

        public IEnumerable<SelectListItem> RaceMaleList { get; set; }

        public IEnumerable<SelectListItem> RaceFemaleList { get; set; }

        public IList<MeasurementsDto> measurementsImperial { get; set; }

        public IList<MeasurementsDto> measurementsMetric { get; set; }

        public CanriskEligibilityModel canriskEligibility { get; set; }

        public int? CompletedBy { get; set; }

        public string languagePreference { get; set; }
    }

    public class ContactUsModel
    {
        public string name { get; set; }

        public string email { get; set; }

        public string meassage { get; set; }

        public string organizationEmail { get; set; }

        public string subject { get; set; }

    }

    public class CanriskEligibilityModel
    {
        public bool isEligible { get; set; }

        public string riskLevel { get; set; }

        public string qualifyText { get; set; }

    }

    public class WellnessDataViewModel
    {
        public int organizationId { get; set; }

        public string dateFormat { get; set; }

        public int teamsBPOrgId { get; set; }

    }

    public class WellnessChartModel
    {
        public bool hasActivePortal { get; set; }

        public int organizationId { get; set; }

        public int teamsBPOrgId { get; set; }
    }

    public class ListWellnessDataModel
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int? TotalRecords { get; set; }
    }

    public class ParticipantAdvancedSearchModel
    {
        public IEnumerable<SelectListItem> OrganizationList { get; set; }

        public IEnumerable<SelectListItem> MedicalConditionList { get; set; }

        public IEnumerable<SelectListItem> ProgramList { get; set; }

        public IEnumerable<SelectListItem> ContactRequirements { get; set; }

        public IEnumerable<SelectListItem> Coaches { get; set; }

        public int Coach { get; set; }

        public string DateFormat { get; set; }
    }

    public class AdvancedSearchModel
    {
        public IList<ListSearchUsers_ResultsDto> Result { get; set; }

        public int TotalRecords { get; set; }

        public string Filter { get; set; }
    }

    public class GetAppointmentsModel
    {
        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }

        public bool? recentlyEnrolled { get; set; }
    }

    public class MissedAppointmentsModel
    {
        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }
    }

    public class AppointmentsListModel
    {
        public List<AppointmentDTO> Appointments { get; set; }

        public DateTime Date { get; set; }
    }

    public class RecentlyEnrolledModel
    {
        public IList<UsersinProgramDto> Users { get; set; }
    }

    public class AdvancedSearchRequestModel
    {
        public string? SearchText { get; set; }

        public int? OrganizationId { get; set; }

        public DateTime? HraStartDate { get; set; }

        public DateTime? HraEndDate { get; set; }

        public int? CoachId { get; set; }

        public string? MedicalCondition { get; set; }

        public byte? ProgramType { get; set; }

        public string? RecentStats { get; set; }

        public int? ContactRequirement { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }

    public class ExternalReportsModel
    {
        public IEnumerable<SelectListItem> reportsList { get; set; }
        public ExternalReportsDto externalReports { get; set; }
    }

    public class SendReportsModel
    {
        public IEnumerable<SelectListItem> reportTypes { get; set; }

        public List<FaxedReportsDto> faxedReports { get; set; }

        public int reportType { get; set; }

        public string FaxNumber { get; set; }

        public bool hasActivePortal { get; set; }
    }

    public class SecurityVerficationModel
    {
        public string PhoneNo { get; set; }

        public string EmailId { get; set; }
    }

    public class TimerReports
    {
        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public int Minutes { get; set; }

        public string Disposition { get; set; }

        public bool Billed { get; set; }
    }

    public class TimerReportsModel
    {
        public IList<TimerReports> timerList { get; set; }
    }
}