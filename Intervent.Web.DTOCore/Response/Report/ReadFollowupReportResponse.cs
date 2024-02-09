namespace Intervent.Web.DTO
{
    public class ReadFollowupReportResponse
    {
        public string UserName { get; set; }

        public string Address { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public DateTime? DOB { get; set; }

        public string Picture { get; set; }

        public DateTime? ProgramStartDate { get; set; }

        public DateTime? AssessmentDate { get; set; }

        public DateTime? BloodTestDate { get; set; }

        public int? FollowUpValidity { get; set; }

        public string FollowupLabSource { get; set; }

        public int? ScheduledCoachingSession { get; set; }

        public int? CompletedCoachingSession { get; set; }

        public List<AssignedKit> AssignedKits { get; set; }

        public string PreviousDate { get; set; }

        public string CurrentDate { get; set; }

        public float? TenYrProbStart { get; set; }

        public float? TenYrProbCurrent { get; set; }

        public float? TenYrLowGoal { get; set; }

        public float? TenYearASCVDStart { get; set; }

        public float? TenYearASCVDCurrent { get; set; }

        public float? TenYearASCVDGoal { get; set; }

        public float? LifetimeASCVDStart { get; set; }

        public float? LifetimeASCVDCurrent { get; set; }

        public float? LifetimeASCVDGoal { get; set; }

        public byte? DidYouFastStart { get; set; }

        public byte? DidYouFastCurrent { get; set; }

        public float? TotalCholStart { get; set; }

        public float? TotalCholCurrent { get; set; }

        public float? HDLStart { get; set; }

        public float? HDLCurrent { get; set; }

        public float? HDLGoal { get; set; }

        public float? LDLStart { get; set; }

        public float? LDLCurrent { get; set; }

        public float? LDLGoal { get; set; }

        public float? TrigStart { get; set; }

        public float? TrigCurrent { get; set; }

        public float? TrigGoal { get; set; }

        public float? WeightStart { get; set; }

        public float? WeightCurrent { get; set; }

        public float? WeightSTGoal { get; set; }

        public float? WeightLTGoal { get; set; }

        public float? Height { get; set; }

        public float? SBPStart { get; set; }

        public float? SBPCurrent { get; set; }

        public float? SBPGoal { get; set; }

        public float? DBPStart { get; set; }

        public float? DBPCurrent { get; set; }

        public float? DBPGoal { get; set; }

        public float? GlucoseStart { get; set; }

        public float? GlucoseCurrent { get; set; }

        public float? GlucoseGoal1 { get; set; }

        public float? GlucoseGoal2 { get; set; }

        public float? A1CStart { get; set; }

        public float? A1CCurrent { get; set; }

        public float? A1CGoal { get; set; }

        public bool? Diabetes { get; set; }

        public float? tobaccoStart { get; set; }

        public float? tobaccoCurrent { get; set; }

        public bool? CholRef { get; set; }

        public bool? BPRef { get; set; }

        public bool? ElevatedBPRef { get; set; }

        public bool? HypertensiveBPRef { get; set; }

        public bool? LdlRef1 { get; set; }

        public bool? LdlRef2 { get; set; }

        public bool? HdlRef { get; set; }

        public int? ASCVDRef { get; set; }

        public bool? TrigRef1 { get; set; }

        public bool? TrigRef2 { get; set; }

        public bool? DiabRef { get; set; }

        public bool? NicRef { get; set; }

        public bool? AspRef { get; set; }

        public List<SurveyQuestionDto> SurveyQuestions { get; set; }

        public bool IsSurveyCompleted { get; set; }

        public int? NoOfWeeks { get; set; }

        public int? PreviousYearScheduledCoachingSession { get; set; }

        public int? PreviousYearCompletedCoachingSession { get; set; }

        public List<AssignedKit> PreviousYearKits { get; set; }
    }

    public class AssignedKit
    {
        public string EducationalTopic { get; set; }

        public DateTime? DateAssigned { get; set; }

        public int? PercentCompleted { get; set; }
    }

}