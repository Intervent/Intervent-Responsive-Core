namespace Intervent.Web.DTO.AWV
{
    public class BiometricsDto
    {
        /// <summary>
        /// Biometrics:A1C
        /// </summary>
        public float? A1C { get; set; }

        /// <summary>
        /// Biometrics:Diastolic blood pressure
        /// </summary>
        public short? DBP { get; set; }

        /// <summary>
        /// Biometrics: Did you fast before taking blood test
        /// 1. Yes 2. No
        /// Default Value: Yes
        /// </summary>
        public byte? Fasting { get; set; }

        /// <summary>
        /// Biometrics:Glucose
        /// </summary>
        public float? Glucose { get; set; }

        /// <summary>
        /// Biometrics:HDL cholesterol
        /// </summary>
        public float? HDL { get; set; }

        /// <summary>
        /// Biometrics: Height (in inches)
        /// </summary>
        public float? Height { get; set; }

        /// <summary>
        /// Biometrics:LDL cholesterol
        /// </summary>
        public float? LDL { get; set; }

        /// <summary>
        /// Biometrics:Systolic blood pressure
        /// </summary>
        public short? SBP { get; set; }

        /// <summary>
        /// Biometrics:Total cholesterol
        /// </summary>
        public float? TotalChol { get; set; }

        /// <summary>
        /// Biometrics:Triglycerides
        /// </summary>
        public float? Trig { get; set; }

        /// <summary>
        /// Biometrics:Waist (in inches)
        /// </summary>
        public float? Waist { get; set; }

        /// <summary>
        /// Biometrics:Weight (in pounds)
        /// </summary>
        public float? Weight { get; set; }

        /// <summary>
        /// Biometrics: BP Date
        /// </summary>
        public DateTime? BPDate { get; set; }

        /// <summary>
        /// Biometrics: LDL Date
        /// </summary>
        public DateTime? LDLDate { get; set; }

        /// <summary>
        /// Biometrics: HDL Date
        /// </summary>
        public DateTime? HDLDate { get; set; }

        /// <summary>
        /// Biometrics: A1C Date
        /// </summary>
        public DateTime? A1CDate { get; set; }

        /// <summary>
        /// Biometrics: Glucose Date
        /// </summary>
        public DateTime? GlucoseDate { get; set; }

        /// <summary>
        /// Biometrics: Total cholesterol Date
        /// </summary>
        public DateTime? TotalCholDate { get; set; }

        /// <summary>
        /// Biometrics: Triglycerides Date
        /// </summary>
        public DateTime? TrigDate { get; set; }

        public Dictionary<string, string> Validate(Dictionary<string, string> validationResults)
        {
            ValidationUtility.Validate(Fasting, 1, 2, "Fasting", validationResults);
            return validationResults;

        }
    }
}
