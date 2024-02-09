namespace Intervent.Web.DTO.AWV
{
    /// <summary>
    /// Annual Wellness Visit Post Data
    /// </summary>
    public class PostAnualWellnessVisitDto
    {

        /// <summary>
        /// Mandatory: Annual Wellness Visit General Information
        /// </summary>
        public AnnulWellnessVisitGeneralInfoDto GeneralInformation { get; set; }

        /// <summary>
        /// Annual Wellness Visit: General Health Information
        /// </summary>
        public GeneralDto GeneralHealth { get; set; }

        /// <summary>
        /// Annual Wellness Visit: Medical History and Providers
        /// </summary>
        public MedicalHistandProvidersDto MedicalHistory { get; set; }

        /// <summary>
        /// Annual Wellness Visit: Preventive Health Screening
        /// </summary>
        public List<PreventiveServicesDto> PreventiveServices { get; set; }

        /// <summary>
        /// Annual Wellness Visit: Health Risk Screening
        /// </summary>
        public HealthRiskScreensDto HealthRiskScreens { get; set; }

        /// <summary>
        /// Annual Wellness Visit: Alcohol Misuse
        /// </summary>
        public AlcoholUseDto AlcoholMisUse { get; set; }

        /// <summary>
        /// Annual Wellness Visit: STD Risk
        /// </summary>
        public STDandProstateRiskDto STDRisk { get; set; }

        /// <summary>
        /// Annual Wellness Visit: Encourage To Quit Tobacco
        /// </summary>
        public TobaccoUseDto QuitTobacco { get; set; }

        /// <summary>
        /// Annual Wellness Visit: Biometrics
        /// </summary>
        public BiometricsDto Biometric { get; set; }

        public Dictionary<string, string> Validate()
        {
            Dictionary<string, string> validatorResult = new Dictionary<string, string>();
            if (GeneralInformation == null)
            {
                validatorResult.Add("GeneralInformation", "Mandatory");
            }
            else
            {
                validatorResult = GeneralInformation.Validate(validatorResult);
                if (GeneralHealth != null)
                    validatorResult = GeneralHealth.Validate(validatorResult);
                if (MedicalHistory != null)
                    validatorResult = MedicalHistory.Validate(validatorResult);
                if (PreventiveServices != null && PreventiveServices.Count > 0)
                {
                    foreach (var service in PreventiveServices)
                        validatorResult = service.Validate(validatorResult);
                }
                if (HealthRiskScreens != null)
                    validatorResult = HealthRiskScreens.Validate(validatorResult);
                if (AlcoholMisUse != null)
                    validatorResult = AlcoholMisUse.Validate(validatorResult);
                if (STDRisk != null)
                    validatorResult = STDRisk.Validate(validatorResult);
                if (QuitTobacco != null)
                    validatorResult = QuitTobacco.Validate(validatorResult);
                if (Biometric != null)
                    validatorResult = Biometric.Validate(validatorResult);
            }
            return validatorResult;
        }

    }

    public static class ValidationUtility
    {
        public static void Validate(byte? value, byte min, byte max, string name, Dictionary<string, string> validationResult)
        {
            if (value.HasValue && (value.Value > max || value.Value < min))
                validationResult.Add(name, string.Format("{0} is not in Range. Min: {1} Max: {2}", value.Value, min, max));
        }
    }

    /// <summary>
    /// Annual Wellness Visit Response
    /// </summary>
    public class AnnualWellnessVisitStatus
    {
        /// <summary>
        /// Token for accessing patient's report
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Link to access to Report page (contains one time token)
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// true for success; false for failure
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Validation Errors
        /// </summary>
        public Dictionary<string, string> ValidationErrors { get; set; }
    }

    public class GetTokenResponse
    {
        /// <summary>
        /// Validation Error
        /// </summary>
        public string ValidationError { get; set; }

        /// <summary>
        /// Token for accessing patient's report
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Link to access to Report page (contains one time token)
        /// </summary>
        public string Link { get; set; }
    }
}