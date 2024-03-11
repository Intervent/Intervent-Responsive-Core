namespace Intervent.DAL
{
    public partial class ListSearchUsers_Result
    {
        public int? Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DOB { get; set; }

        public string Email { get; set; }

        public byte? Gender { get; set; }

        public string? UniqueId { get; set; }

        public string? ProfilePicture { get; set; }

        public int? HRAId { get; set; }

        public DateTime? HRACompleteDate { get; set; }

        public int? PortalID { get; set; }

        public string Organization { get; set; }

        public int OrganizationId { get; set; }

        public string? RiskCode { get; set; }

        public float? UOMRisk { get; set; }

        public float? IVRisk { get; set; }

        public DateTime? NextAppointment { get; set; }

        public DateTime? LastAppointment { get; set; }

        public string? CoachName { get; set; }

        public byte? ProgramType { get; set; }

        public int? Records { get; set; }

        public string? HRAPageSeqDone { get; set; }

        public string? HRAPageSeq { get; set; }

        public int? Glucose { get; set; }

        public DateTime? LastUpdatedGlucose { get; set; }

        public int? Systolic { get; set; }

        public int? Diastolic { get; set; }

        public DateTime? LastUpdatedBP { get; set; }

        public DateTime? AlertCreatedTime { get; set; }

        public string? ContactRequirement { get; set; }

        public string? CoachingMessage { get; set; }

        public string? SelfHelpMessage { get; set; }

        public int? CoachingOutBound { get; set; }

        public int? SelfHelpOutBound { get; set; }

        public int? AppointmentWithIn { get; set; }

        public int? AlertValue { get; set; }

        public string? MedicalPlanCode { get; set; }

        public DateTime? MedicalPlanEndDate { get; set; }

        public DateTime? LastAccessedOn { get; set; }
    }
}
