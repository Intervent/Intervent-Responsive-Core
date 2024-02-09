namespace Intervent.Web.DTO
{
    public class ParticipantProfile_ResultDto
    {
        public int UserId { get; set; }
        public DateTime? DOB { get; set; }
        public string Address { get; set; }
        public string HomeNumber { get; set; }
        public string WorkNumber { get; set; }
        public string Email { get; set; }

        public byte? Unit { get; set; }
        public string UniqueId { get; set; }
        public string LanguagePreference { get; set; }
        public string StateName { get; set; }
        public string TimeZoneId { get; set; }
        public string TimeZone { get; set; }
        public int? OrgId { get; set; }
        public string OrgName { get; set; }
        public string OrgCode { get; set; }
        public string ContactNum { get; set; }
        public string ContactEmail { get; set; }
        public bool OwnCoach { get; set; }
        public bool? DeclinedEnrollment { get; set; }
        public bool? DoNotTrack { get; set; }
        public int? PortalId { get; set; }
        public int? HRAId { get; set; }
        public float? Weight { get; set; }
        public DateTime? CollectedOn { get; set; }
        public string WellnessVision { get; set; }
        public int? UPId { get; set; }
        public byte? ProgramType { get; set; }
        public string CoachFirst { get; set; }
        public string CoachLast { get; set; }
        public int? NotesId { get; set; }
        public byte? FollowUp { get; set; }
        public byte? Gender { get; set; }
        public int? PPId { get; set; }
        public DateTime? NotesDate { get; set; }
        public bool? Pinned { get; set; }
        public string Note { get; set; }
        public string NoteFirstName { get; set; }
        public string NoteLastName { get; set; }
        public string UserStatus { get; set; }
        public string MedicalPlanCode { get; set; }
        public string BusinessUnit { get; set; }
        public string TobaccoFlag { get; set; }
        public string ProgramName { get; set; }
        public bool? ProgramActiveStatus { get; set; }
        public DateTime? MedicalPlanEndDate { get; set; }
        public byte? IntegrationWith { get; set; }
        public int? CanriskScore { get; set; }
        public bool? CompIntroKitsOnTime { get; set; }
        public bool? CoachingEnabled { get; set; }
        public DateTime? CoachingExpirationDate { get; set; }
        public DateTime? CanriskCompletedOn { get; set; }
        public int? StateId { get; set; }
        public int? EligibilityId { get; set; }
        public DateTime? StudyEnrollmentDate { get; set; }
        public int? CRMId { get; set; }
    }
}
