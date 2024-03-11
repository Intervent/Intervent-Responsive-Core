namespace Intervent.Web.DTO
{
    public class PortalDto
    {
        public int OrganizationId { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public int? HRAValidity { get; set; }

        public byte? HasHRA { get; set; }

        public string HAPageSeq { get; set; }

        public bool HasCoachingProgram { get; set; }

        public bool? HRAforCoachingProgram { get; set; }

        public bool? FollowUpforCoaching { get; set; }

        public bool HasSelfHelpProgram { get; set; }

        public bool? HRAforSelfHelpProgram { get; set; }

        public bool? FollowUpforSelfHelp { get; set; }

        public bool Active { get; set; }

        public bool AWV { get; set; }

        public bool LabIntegration { get; set; }

        public string LabProceduresStr { get; set; }

        public bool CampaignCalls { get; set; }

        public bool AppointmentCalls { get; set; }

        public bool GINAQuestion { get; set; }

        public bool CACScanQuestion { get; set; }

        public bool ShowProgramOption { get; set; }

        public bool SelfScheduling { get; set; }

        public bool ShowPricing { get; set; }

        public bool ClientNameInReport { get; set; }

        public IList<ProgramsinPortalDto> ProgramsinPortal { get; set; }

        public IList<EligibilityDto> Eligibilities { get; set; }

        public bool TrackingCalls { get; set; }

        public int NoOfTrackingCalls { get; set; }

        public bool AutoImmune { get; set; }

        public bool OwnCoach { get; set; }

        public bool MailScoreCard { get; set; }

        public bool CarePlan { get; set; }

        public byte? CarePlanType { get; set; }

        public int? NoProgDays { get; set; }

        public string IncompleteHRA { get; set; }

        public string HRAOnly { get; set; }

        public string HRAandBiometrics { get; set; }

        public string FirstCoaching { get; set; }

        public string CompletedCoaching { get; set; }

        public string FirstTimeHRA { get; set; }

        public string ReportText { get; set; }

        public string ProgramText { get; set; }

        public string ContactText { get; set; }

        public string EmployerIncentiveText { get; set; }

        public string LabCorpAttachment { get; set; }

        public string DoctorOfficeAttachment { get; set; }

        public byte? HRAVer { get; set; }

        public byte? EligibilityFormat { get; set; }

        public bool usePreviousLabs { get; set; }

        public string EligibilityFolderPath { get; set; }

        public bool EligibilityImportLoadFlag { get; set; }

        public bool EligibilityTerminateFlag { get; set; }

        public bool ProcessLivongoCodes { get; set; }

        public bool ProcessInterventCodes { get; set; }

        public string ClaimsDirectoryPath { get; set; }

        public string ValidLabs { get; set; }

        public string PatientReleaseForm { get; set; }

        public string MedicalClearanceForm { get; set; }

        public string KnowYourNumbersForm { get; set; }

        public string TestimonialForm { get; set; }

        public string TobaccoReleaseForm { get; set; }

        public bool AssignPrograms { get; set; }

        public string languagesStr { get; set; }

        public string specializationsStr { get; set; }

        public string followupTypeStr { get; set; }

        public string selfhelpFollowupTypeStr { get; set; }

        public string followuplabStr { get; set; }

        public string selfhelpfollowuplabStr { get; set; }

        public string CoachingConditionsStr { get; set; }

        public bool ShowPostmenopausal { get; set; }

        public bool AllowCardiacQuestion { get; set; }

        public byte? EligtoIntuity { get; set; }

        public OrganizationDto Organization { get; set; }

        public List<LanguagesDto> Languages { get; set; }

        public List<SpecializationDto> Specializations { get; set; }

        public List<FollowUpTypeDto> FollowUpTypes { get; set; }

        public List<PortalFollowUpDto> PortalFollowUps { get; set; }

        public int? FollowUpValidity { get; set; }

        public string CampaignStartDate { get; set; }

        public string CampaignEndDate { get; set; }

        public bool NeedCareplanApproval { get; set; }

        public string CareplanPath { get; set; }

        public bool? ShowTimeTracker { get; set; }

        public byte ProviderDetails { get; set; }

        public bool SelfEnterLabs { get; set; }

        public bool FaxReports { get; set; }

        public bool KitAlert { get; set; }

        public string WelcomeText { get; set; }

        public List<PortalCoachingConditionsDto> PortalCoachingConditions { get; set; }

        public List<PortalLabProcedureDto> PortalLabProcedures { get; set; }

        public virtual List<PortalIncentiveDto> PortalIncentives { get; set; }
    }
}