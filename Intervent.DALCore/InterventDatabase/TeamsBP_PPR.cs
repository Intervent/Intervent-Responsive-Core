namespace Intervent.DAL
{
    public partial class TeamsBP_PPR
    {
        public int Id { get; set; }

        public int WellnessId { get; set; }

        public byte? MonitoringBP { get; set; }

        public byte? TakingBPMed { get; set; }

        public byte? HealthyDiet { get; set; }

        public byte? ManagingWeight { get; set; }

        public byte? LimitingAlcohol { get; set; }

        public byte? QuitSmoking { get; set; }

        public string? OtherGoals { get; set; }

        public bool? BPTriage { get; set; }

        public bool? MonitoredBP { get; set; }

        public int? MonitoredBPDays { get; set; }

        public string? NotMonitoredReason { get; set; }

        public string? NotMonitoredReasonText { get; set; }

        public string? MonitoredBPHelpful { get; set; }

        public string? MonitoredBPHelpfulText { get; set; }

        public bool? ChangeInBPMonitoringGoal { get; set; }

        public string? NewBPMonitoringGoal { get; set; }

        public bool? CurrentBPMed { get; set; }

        public bool? MedicationChanges { get; set; }

        public string? MedListChanges { get; set; }

        public int? MissedMed { get; set; }

        public bool? TakingMedication { get; set; }

        public string? ReasonNotTakingMed { get; set; }

        public string? ReasonNotTakingMedText { get; set; }

        public bool? ChangeInBPMedPrescribed { get; set; }

        public string? NewBPMedPrescribed { get; set; }

        public bool? AvgStepsDayReviewed { get; set; }

        public bool? StepsGoalAttained { get; set; }

        public bool? AvgMinutesDayReviewed { get; set; }

        public bool? MinutesGoalAttained { get; set; }

        public string? YAcheivedGoal { get; set; }

        public string? YAcheivedGoalText { get; set; }

        public string? YNotAcheivedGoal { get; set; }

        public string? YNotAcheivedGoalText { get; set; }

        public bool? ChangeInPAGoal { get; set; }

        public string? NewPAGoal { get; set; }

        public int? MinGoal { get; set; }

        public int? StepGoal { get; set; }

        public bool? FollowedLSD { get; set; }

        public string? ReasonNotFollowedLSD { get; set; }

        public string? ReasonNotFollowedLSDText { get; set; }

        public bool? ChangeInHDGoal { get; set; }

        public string? NewHDGoal { get; set; }

        public bool? ReviewedProblemList { get; set; }

        public string? ReviewedProblemListText { get; set; }

        public bool? ReviewedNeurology { get; set; }

        public bool? HasVisitedED { get; set; }

        public int? EDVisitsNumber { get; set; }

        public string? EDDateOfVisit { get; set; }

        public string? EDVisitReason { get; set; }

        public bool? WasAdmittedHospital { get; set; }

        public int? HospitalVisitsNumber { get; set; }

        public string? DateOfAdmit { get; set; }

        public string? DaysInHospital { get; set; }

        public string? HospitalizationReason { get; set; }

        public bool? HasFallen { get; set; }

        public int? NumberOfFalls { get; set; }

        public string? InjuredWhenFell { get; set; }

        public string? HowSeriousWasInjury { get; set; }

        public bool? ReferredByProvider { get; set; }

        public bool? NeedsProviderReferral { get; set; }

        public bool? AttendingAsScheduled { get; set; }

        public string? NotAttendingReason { get; set; }

        public string? NotAttendingReasonText { get; set; }

        public bool? CommunityStrokeRecovery { get; set; }

        public string? CommunityUsing { get; set; }

        public bool? NewCommunityHelp { get; set; }

        public string? CommunityNeeded { get; set; }

        public bool? ChangeManagingWeight { get; set; }

        public string? NewWeightGoal { get; set; }

        public bool? ChangeManagingStress { get; set; }

        public string? NewStressGoal { get; set; }

        public bool? ChangeLimitingAlcohol { get; set; }

        public string? NewAlcoholGoal { get; set; }

        public bool? ChangeQuitSmoking { get; set; }

        public string? NewSmokingGoal { get; set; }

        public string? Notes { get; set; }

        public virtual WellnessData WellnessData { get; set; }
    }
}
