namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("WellnessData")]
    public partial class WellnessData
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WellnessData()
        {
            BillingNotes = new HashSet<BillingNote>();
            TeamsBP_PPR = new HashSet<TeamsBP_PPR>();
        }

        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime? CollectedOn { get; set; }

        public int? CollectedBy { get; set; }

        public float? Weight { get; set; }

        public byte? sameDevice { get; set; }

        public short? SBP { get; set; }

        public short? DBP { get; set; }

        public float? waist { get; set; }

        public short? Pulse { get; set; }

        public byte? TakeVitamins { get; set; }

        public byte? TakeMeds { get; set; }

        public byte? MissMeds { get; set; }

        public byte? ChangeMeds { get; set; }

        public byte? Allergyreaction { get; set; }

        public short? ExerMin { get; set; }

        public byte? ExerInt { get; set; }

        public byte? HealthyEating { get; set; }

        public byte? PhysicallyActive { get; set; }

        public byte? ManageStress { get; set; }

        public byte? Motivation { get; set; }

        public byte? CurrentSmoker { get; set; }

        public short? SmokedPerDay { get; set; }

        public DateTime? YQuitDate { get; set; }

        public byte? YContractSign { get; set; }

        public DateTime? NQuitDate { get; set; }

        public byte? ManWithdrawSymp { get; set; }

        public byte? UsingNRT { get; set; }

        public byte? Gum { get; set; }

        public byte? Patch { get; set; }

        public byte? Lozenge { get; set; }

        public byte? Inhaler { get; set; }

        public byte? Spray { get; set; }

        public byte? UsingBupropion { get; set; }

        public byte? UsingVarenicline { get; set; }

        [StringLength(100)]
        public string? UsingOtherDrug { get; set; }

        public int? SourceHRA { get; set; }

        public string? WellnessVision { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? SourceFollowUp { get; set; }

        public bool? isPregnant { get; set; }

        public DateTime? DueDate { get; set; }

        public bool? isDiabetic { get; set; }

        public byte? DiabeticType { get; set; }

        public byte? UpdateHealth { get; set; }

        public byte? Steroid { get; set; }

        public byte? Pancreatitis { get; set; }

        public byte? CancerTreated { get; set; }

        public byte? Hemoglobinopathy { get; set; }

        public byte? StudyDrug { get; set; }

        public byte? GlucoseLowering { get; set; }

        public float? GlucoseFrom { get; set; }

        public float? GlucoseTo { get; set; }


        public byte? WeightLoss { get; set; }

        public byte? TakingMedication { get; set; }

        public byte? MedicationChanges { get; set; }

        public byte? SideEffects { get; set; }

        public string? MedicationComments { get; set; }

        public byte? TakingInsulin { get; set; }

        public string? DiabetesComments { get; set; }

        public byte? Discomfort { get; set; }

        public byte? DiscomfortReason { get; set; }

        public byte? BreathShortness { get; set; }

        public byte? ShortnessReason { get; set; }

        public byte? Dizzyness { get; set; }

        public byte? DizzynessReason { get; set; }

        public byte? FastHeartRate { get; set; }

        public byte? FastHeartRateReason { get; set; }

        public byte? Swelling { get; set; }

        public byte? UnusualSymptoms { get; set; }

        public string? CardiacComments { get; set; }

        public byte? WeightIncrease { get; set; }

        public byte? Urination { get; set; }

        public byte? Sputum { get; set; }

        public byte? BreathatNight { get; set; }

        public byte? BreathatLieDown { get; set; }

        public string? HeartFailureComments { get; set; }

        public byte? LegDiscomfort { get; set; }

        public int? DiscomfortMinutes { get; set; }

        public byte? Claudication { get; set; }

        public int? BeforePainMinutes { get; set; }

        public byte? FootWounds { get; set; }

        public string? PADComments { get; set; }

        public byte? DidDocReqGlucoseTest { get; set; }

        public byte? NoOfGlucTestRequested { get; set; }

        public byte? NoOfGlucTestDone { get; set; }

        public byte? Symptoms { get; set; }

        public string? SymptomsComments { get; set; }

        public float? A1C { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }

        public virtual User User2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BillingNote> BillingNotes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TeamsBP_PPR> TeamsBP_PPR { get; set; }
    }
}
