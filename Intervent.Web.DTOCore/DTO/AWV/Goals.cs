namespace Intervent.Web.DTO.AWV
{
    public enum Risk
    {
        HR = 1,
        LR = 2,
        ID = 3,
    }

    public enum Track
    {
        OnTrack = 1,
        NotOnTrack = 2,
        NotApplicable = 3,
        Optional = 4,
        Indeterminate = 5
    }


    public static class Constants
    {
        public static readonly string Now = "NOW";
        public static readonly string NotApplicable = "NA";
        public static readonly string YearlyFlu = "YF";
        public static readonly string NotNeeded = "NN";
        public static readonly string Physician = "P";
        public static readonly string NextPhysician = "NP";

        public static readonly string Separator = "~";
    }

    public class GoalsDto
    {
        public int Id { get; set; }

        public string DrComments { get; set; }

        public string PrintToken { get; set; }

        public UserDto User { get; set; }

        public AnnulWellnessVisitGeneralInfoDto AWV { get; set; }

        public BiometricsDto Biometrics { get; set; }
        //Personal
        #region Personal
        public byte? Cardiovascular { get; set; }

        public byte? HighBP { get; set; }

        public byte? CholTrig { get; set; }

        public byte? Diabetes { get; set; }

        public byte? Cancer { get; set; }

        public byte? AsthmaCOPD { get; set; }

        public byte? OtherChronic { get; set; }
        #endregion

        //Family
        #region Family
        public byte? FHCardiovascular { get; set; }

        public byte? FHDiabetes { get; set; }

        public byte? FHCancer { get; set; }
        #endregion

        //Psychosocial
        #region PsychoSocial
        public byte? Depression { get; set; }

        public byte? LifeSatisfication { get; set; }

        public byte? AngerRisk { get; set; }

        public byte? LonelyRisk { get; set; }

        public byte? BodilyPain { get; set; }

        public byte? Fatigue { get; set; }

        public byte? Cognition { get; set; }

        #endregion

        //Lifestyle/Behavioral Factors
        #region Lifestyle
        public byte? TobaccoUse { get; set; }

        public byte? PhysicalActivity { get; set; }

        public byte? Nutrition { get; set; }

        public byte? Alcohol { get; set; }

        public byte? Stress { get; set; }

        public byte? Safety { get; set; }

        public byte? OralHealth { get; set; }

        public byte? SexHealth { get; set; }
        #endregion

        //Functional ability
        #region Functional Ability
        public byte? DailyActivity { get; set; }

        public byte? Falls { get; set; }

        public byte? HearingImp { get; set; }

        public byte? VisionImp { get; set; }

        public byte? UrinaryInc { get; set; }

        public byte? Prostate { get; set; }
        #endregion

        //Clinical Measurement
        #region Clinical Measurement
        public byte? Weight { get; set; }

        public byte? BP { get; set; }

        public byte? Cholesterol { get; set; }

        public byte? GlucA1C { get; set; }

        #endregion

        //Preventive Screening
        #region Preventive Screening
        public byte? Colorectal { get; set; }

        public string ColorectalRecDate { get; set; }

        public byte? PapTest { get; set; }

        public string PapTestRecDate { get; set; }

        public byte? Mammogram { get; set; }

        public string MammogramRecDate { get; set; }

        public byte? PSAProstate { get; set; }

        public string PSAProstateRecDate { get; set; }

        public byte? BPScreening { get; set; }

        public string BPScreeningRecDate { get; set; }

        public byte? Lipoproteins { get; set; }

        public string LipoproteinsRecDate { get; set; }

        public byte? DiabetesScreening { get; set; }

        public string DiabetesScreeningRecDate { get; set; }

        public byte? Obesity { get; set; }

        public string ObesityRecDate { get; set; }

        //public byte? Dental { get; set; }

        public byte? Vision { get; set; }

        public string VisionRecDate { get; set; }

        public byte? Hearing { get; set; }

        public string HearingRecDate { get; set; }

        public byte? Aortic { get; set; }

        public string AorticRecDate { get; set; }

        public byte? Osteoporosis { get; set; }

        public string OsteoporosisRecDate { get; set; }
        public byte? LungCancer { get; set; }

        public string LungCancerRecDate { get; set; }

        public byte? HIV { get; set; }

        public string HIVRecDate { get; set; }
        #endregion

        //Immunization
        #region Immunization

        public byte? Tetanus { get; set; }

        public string TetanusRecDate { get; set; }

        public byte? Pneumonia { get; set; }

        public string PneumoniaRecDate { get; set; }

        public byte? Flu { get; set; }

        public string FluRecDate { get; set; }

        //public byte? MMR { get; set; }

        //public byte? Chickenpox { get; set; }

        public byte? Shingles { get; set; }

        public string ShinglesRecDate { get; set; }

        //public byte? HPV { get; set; }

        public byte? HepatitisB { get; set; }

        public string HepatitisBRecDate { get; set; }
        #endregion

        #region KeyAction Steps
        public string Lifestyle { get; set; }

        public string PreventiveCare { get; set; }

        public string Other { get; set; }
        #endregion

        //Health Measurements
        #region Health Measuremnet Goals

        public float? LtWt { get; set; }

        //public float? LtTrig { get; set; }
        //hardcode to 150 since it is only for US

        public float? LtSBP { get; set; }

        public float? LtDBP { get; set; }

        // public float? LtLdl { get; set; }

        //public float? LtHdl { get; set; }

        public float? LtGluc1 { get; set; }

        public float? LtGluc2 { get; set; }

        public bool ASCVD { get; set; }


        public float? TenYrProb { get; set; }

        public float? TenYrLow { get; set; }
        //TODO: bp treat med
        public float? TenYearASCVDGoal { get; set; }

        public float? TenYearASCVD { get; set; }

        public float? LifetimeASCVD { get; set; }

        public float? LifetimeASCVDGoal { get; set; }
        #endregion

        public string SafetyGoals { get; set; }

        public string DrReferralCode { get; set; }
        #region Programs
        public byte? CardioSmart { get; set; }

        public byte? DiabeticMgnt { get; set; }

        public byte? WeightMgnt { get; set; }

        public byte? TobaccoCes { get; set; }

        public byte? NutritionalMgnt { get; set; }
        #endregion
    }

}
