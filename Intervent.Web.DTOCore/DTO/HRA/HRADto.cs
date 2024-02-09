namespace Intervent.Web.DTO
{
    public class HRADto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int PortalId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? CompleteDate { get; set; }

        public string HAPageSeqDone { get; set; }

        public bool GoalsGenerated { get; set; }

        public string RiskCode { get; set; }

        public float? UOMRisk { get; set; }

        public float? IVRisk { get; set; }

        public float? WellnessScore { get; set; }

        public int? Age { get; set; }

        public string Language { get; set; }

        public int? CreatedBy { get; set; }

        public int? ADAScore { get; set; }

        public double? PAISummary { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public MedicalConditionsDto MedicalCondition { get; set; }

        public OtherRiskFactorsDto OtherRiskFactors { get; set; }

        public HSPDto HSP { get; set; }

        public HealthNumbersDto HealthNumbers { get; set; }

        public InterestsDto Interest { get; set; }

        public ExamsandShotsDto Exams { get; set; }

        public HRAGoalsDto Goals { get; set; }

        public PortalDto Portal { get; set; }

        public UserDto User { get; set; }

        public FollowUp_HealthNumbersDto FollowupHealthNumber { get; set; }

        public IList<ReportFeedbackDto> ReportFeedbacks { get; set; }
    }
}