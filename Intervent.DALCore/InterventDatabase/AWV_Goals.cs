namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class AWV_Goals
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public DateTime DateCreated { get; set; }

        public byte? Cardiovascular { get; set; }

        public byte? HighBP { get; set; }

        public byte? CholTrig { get; set; }

        public byte? Diabetes { get; set; }

        public byte? Cancer { get; set; }

        public byte? AsthmaCOPD { get; set; }

        public byte? OtherChronic { get; set; }

        public byte? FHCardiovascular { get; set; }

        public byte? FHDiabetes { get; set; }

        public byte? FHCancer { get; set; }

        public byte? Depression { get; set; }

        public byte? LifeSatisfication { get; set; }

        public byte? AngerRisk { get; set; }

        public byte? LonelyRisk { get; set; }

        public byte? BodilyPain { get; set; }

        public byte? Fatigue { get; set; }

        public byte? Cognition { get; set; }

        public byte? TobaccoUse { get; set; }

        public byte? PhysicalActivity { get; set; }

        public byte? Nutrition { get; set; }

        public byte? Alcohol { get; set; }

        public byte? Stress { get; set; }

        public byte? Safety { get; set; }

        public byte? OralHealth { get; set; }

        public byte? SexHealth { get; set; }

        public byte? DailyActivity { get; set; }

        public byte? Falls { get; set; }

        public byte? HearingImp { get; set; }

        public byte? VisionImp { get; set; }

        public byte? UrinaryInc { get; set; }

        public byte? Prostate { get; set; }

        public byte? Weight { get; set; }

        public byte? BP { get; set; }

        public byte? Cholesterol { get; set; }

        public byte? GlucA1C { get; set; }

        public byte? Colorectal { get; set; }

        public byte? PapTest { get; set; }

        public byte? Mammogram { get; set; }

        public byte? PSAProstate { get; set; }

        public byte? BPScreening { get; set; }

        public byte? Lipoproteins { get; set; }

        public byte? DiabetesScreening { get; set; }

        public byte? Obesity { get; set; }

        public byte? Vision { get; set; }

        public byte? Hearing { get; set; }

        public byte? Aortic { get; set; }

        public byte? Osteoporosis { get; set; }

        public byte? LungCancer { get; set; }

        public byte? HIV { get; set; }

        public byte? Tetanus { get; set; }

        public byte? Pneumonia { get; set; }

        public byte? Flu { get; set; }

        public byte? Shingles { get; set; }

        public byte? HepatitisB { get; set; }

        [StringLength(124)]
        public string? ColorectalRecDate { get; set; }

        [StringLength(124)]
        public string? PapTestRecDate { get; set; }

        [StringLength(124)]
        public string? MammogramRecDate { get; set; }

        [StringLength(124)]
        public string? PSAProstateRecDate { get; set; }

        [StringLength(124)]
        public string? LipoproteinsRecDate { get; set; }

        [StringLength(124)]
        public string? DiabetesScreeningRecDate { get; set; }

        [StringLength(124)]
        public string? VisionRecDate { get; set; }

        [StringLength(124)]
        public string? AorticRecDate { get; set; }

        [StringLength(124)]
        public string? OsteoporosisRecDate { get; set; }

        [StringLength(124)]
        public string? Lifestyle { get; set; }

        [StringLength(124)]
        public string? PreventiveCare { get; set; }

        [StringLength(124)]
        public string? Other { get; set; }

        [StringLength(64)]
        public string? SafetyGoals { get; set; }

        [StringLength(64)]
        public string? BPScreeningRecDate { get; set; }

        [StringLength(64)]
        public string? ObesityRecDate { get; set; }

        [StringLength(64)]
        public string? HearingRecDate { get; set; }

        [StringLength(64)]
        public string? LungCancerRecDate { get; set; }

        [StringLength(64)]
        public string? HIVRecDate { get; set; }

        [StringLength(64)]
        public string? TetanusRecDate { get; set; }

        [StringLength(64)]
        public string? PneumoniaRecDate { get; set; }

        [StringLength(64)]
        public string? FluRecDate { get; set; }

        [StringLength(64)]
        public string? ShinglesRecDate { get; set; }

        [StringLength(64)]
        public string? HepatitisBRecDate { get; set; }

        public byte? CardioSmart { get; set; }

        public byte? WeightMgnt { get; set; }

        public byte? TobaccoCes { get; set; }

        public byte? NutritionalMgnt { get; set; }

        public byte? DiabeticMgnt { get; set; }

        [StringLength(64)]
        public string? DrReferralCode { get; set; }

        public float? LtWt { get; set; }

        public float? LtSBP { get; set; }

        public float? LtDBP { get; set; }

        public float? LtLdl { get; set; }

        public float? LtHdl { get; set; }

        public float? LtGluc1 { get; set; }

        public float? LtGluc2 { get; set; }

        public float? TenYrLow { get; set; }

        public float? TenYearASCVDGoal { get; set; }

        public float? TenYearASCVD { get; set; }

        public float? LifetimeASCVD { get; set; }

        public float? LifetimeASCVDGoal { get; set; }

        public float? TenYrProb { get; set; }

        public bool ASCVD { get; set; }

        public virtual AWV AWV { get; set; }
    }
}
