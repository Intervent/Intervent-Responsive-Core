using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class ProgramsinPortalsModel
    {
        public bool hasActivePortal { get; set; }

        public IList<ProgramsinPortalDto> ProgramsinPortals { get; set; }

        public bool userinProgram { get; set; }

        public int? userinProgramId { get; set; }

        public int? currentProgramId { get; set; }

        public bool taskforProgram { get; set; }

        public bool? failed { get; set; }

        public int participantId { get; set; }

        public int userId { get; set; }

        public bool showPricing { get; set; }
    }

    public class KitsinProgramModel
    {
        public IList<KitsinProgramDto> kitsinProgram { get; set; }
    }

    public class UserProgramModel
    {
        public bool IsEligibileToEnroll { get; set; }

        public bool hasActivePortal { get; set; }
    }


    public class UserinProgramModel
    {
        public UsersinProgramDto userinProgram { get; set; }

        public VitalsLogDto dailyVitals { get; set; }

        public bool hasPendingVitals { get; set; }

        public IList<KitsinUserProgramGoalDto> kitsinUserProgramGoals { get; set; }

        public string FutureAppointmentDate { get; set; }

        public string timeZone { get; set; }

        public float SBP { get; set; }

        public float DBP { get; set; }

        public IList<MeasurementsDto> Measurements { get; set; }

        public float WeightDiff { get; set; }

        public float Weight { get; set; }

        public string WellnessVision { get; set; }

        public List<UsersinProgramDto> pastPrograms { get; set; }

        public DateTime? FollowupCompletedDate { get; set; }

        public bool IsSurveyCompleted { get; set; }

        public bool PartiallyCompleted { set; get; }

        public List<UserFormDto> userForms { set; get; }

        public string PatientReleaseForm { get; set; }

        public string MedicalClearanceForm { get; set; }

        public string KnowYourNumbersForm { get; set; }

        public string TestimonialForm { get; set; }

        public string TobaccoReleaseForm { get; set; }

        public string FoodList { get; set; }

        public string CoachImage { get; set; }

        public Step StepName { get; set; }

        public List<Step> StepNames { get; set; }

        public KitsDto EduKit { get; set; }

        public bool IsCoachingProgramAvailable { get; set; }

        public List<SurveyQuestionDto> SurveyQuestions { get; set; }
        public bool IsVitalsCompletionIncentive { get; set; }

        public String VitalsCompletionPoints { get; set; }

        public bool IsKitCompletionIncentive { get; set; }

        public String KitCompletionPoints { get; set; }

        public bool IsKitGoalCompletionIncentive { get; set; }

        public String KitGoalCompletionPoints { get; set; }

        public bool IsPortalIncentivePresent { get; set; }

        public bool ShowFormsTab { get; set; }

        public ProgramWellnessDataModel ProgramWellnessData { get; set; }

        public bool showSelfScheduling { get; set; }

        public string dob { get; set; }

        public bool hasActivePortal { get; set; }

        public int? assignedFollowUp { get; set; }

        public int gender { get; set; }

        public int? hraId { get; set; }

        public string hraCompleteDate { get; set; }

        public int? hasHRA { get; set; }

        public double incentivePoints { get; set; }
    }
    public class ProgramWellnessDataModel
    {
        public IList<MeasurementsDto> Measurements { get; set; }

        public WellnessDataDto wellnessData { get; set; }

        public float? SBP { get; set; }

        public float? DBP { get; set; }

        public string WeightStatus { get; set; }

        public float WeightDiff { get; set; }

        public float? Weight { get; set; }

        public float? Waist { get; set; }

        public string Glucose { get; set; }

        public string WellnessVision { get; set; }

        public int wellnessDataId { get; set; }

        public bool hasActivePortal { get; set; }

        public bool isTeamsBP { get; set; }
    }

    public class AddProgramModel
    {
        public IEnumerable<SelectListItem> Coaches { get; set; }

        public int CoachId { get; set; }

        public IEnumerable<SelectListItem> ProgramsinPortals { get; set; }

        public IEnumerable<SelectListItem> Language { get; set; }

        public int PrograminPortalId { get; set; }

        public string hraRisk { get; set; }

        public int? suggestedCoach { get; set; }

        public string eligible { get; set; }

        public IEnumerable<SelectListItem> Programs { get; set; }
    }

    public class AddKittoUserProgramModel
    {
        public IEnumerable<SelectListItem> Kits { get; set; }

        public int KitId { get; set; }
    }

    public class UpdateUserProgram
    {
        public int UsersinProgramId { get; set; }

        public IEnumerable<SelectListItem> Coaches { get; set; }

        public int? CoachId { get; set; }

        public IEnumerable<SelectListItem> ProgramsinPortals { get; set; }

        public int PrograminPortalId { get; set; }

        public IEnumerable<SelectListItem> InactiveReason { get; set; }

        public int InactiveReasonId { get; set; }

        public string LanguagePreference { get; set; }

        public bool disableProgramChange { get; set; }

        public IEnumerable<SelectListItem> Language { get; set; }
    }

    public class ActivateUserProgram
    {
        public int UsersinProgramId { get; set; }
    }

    public class AppointmentCallTemplateModel
    {
        public IEnumerable<AppointmentCallTemplate> CallTemplates { get; set; }
    }

    public class AppointmentCallTemplate
    {
        public string TemplateName { get; set; }

        public int TemplateId { get; set; }

        public int NoOfWeeks { get; set; }

        public int NoOfCalls { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<AppointmentCallIntervalModel> CallIntervals { get; set; }
    }

    public class AppointmentCallIntervalModel
    {
        public IEnumerable<AppointmentCallInterval> CallIntervals { get; set; }
    }

    public class AppointmentCallInterval
    {
        public string CallIntervalId { get; set; }

        public string TemplateId { get; set; }

        public string CallNumber { get; set; }

        public string IntervalInDays { get; set; }
    }

    public class SuggestedCoachingDateModel
    {
        public Dictionary<int, DateTime> suggestedCoachingDates { get; set; }

        public string dateFormat { get; set; }
    }
}