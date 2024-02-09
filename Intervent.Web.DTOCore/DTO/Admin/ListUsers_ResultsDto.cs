namespace Intervent.Web.DTO
{
    public class ListUsers_ResultsDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DOB { get; set; }

        public string Email { get; set; }

        public string UniqueId { get; set; }

        public bool? DeclinedEnrollment { get; set; }

        public bool? DoNotTrack { get; set; }

        public string Organization { get; set; }

        public string HAPageSeq { get; set; }

        public DateTime? LastAccessedOn { get; set; }

        public string RiskCode { get; set; }

        public string HAPageSeqDone { get; set; }

        public float? UOMRisk { get; set; }

        public float? IVRisk { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? CompleteDate { get; set; }

        public DateTime? Date { get; set; }

        public byte? ProgramType { get; set; }

        public int? PortalId { get; set; }

        public int? HRAId { get; set; }

        public string MedicalPlanCode { get; set; }

        public DateTime? MedicalPlanEndDate { get; set; }
    }
}
