namespace Intervent.Web.DTO.AWV
{
    /// <summary>
    /// Medicare Preventive Services
    /// </summary>
    public class PreventiveServicesDto
    {
        /// <summary>
        /// Service Last Received (UTC Format)
        /// </summary>
        public DateTime? LastReceived { get; set; }

        /// <summary>
        /// Recommend
        /// 1 - Yes
        /// </summary>
        public byte? Recommend { get; set; }

        /// <summary>
        /// Testing
        /// 1 - Yes
        /// </summary>
        public byte? Testing { get; set; }

        /// <summary>
        /// Mandatory: Service Type
        /// </summary>
        public PreventiveServicesType? Type { get; set; }

        public Dictionary<string, string> Validate(Dictionary<string, string> validateResult)
        {
            ValidationUtility.Validate(Recommend, 1, 1, "Recommend", validateResult);
            ValidationUtility.Validate(Testing, 1, 1, "Testing", validateResult);
            if (!Type.HasValue)
                validateResult.Add("PreventiveServicesDto.Type", "Type mandatory");
            return validateResult;
        }
    }

    /// <summary>
    /// Supported Service Type
    /// </summary>
    public enum PreventiveServicesType
    {
        /// <summary>
        /// Welcome to Medicare (IPPE)
        /// </summary>
        IPPE = 1,
        /// <summary>
        /// Annual Wellness Visit
        /// </summary>
        AWV = 2,
        /// <summary>
        /// Influenza (Flu)
        /// </summary>
        Influenza = 3,
        /// <summary>
        /// Hepatitis B
        /// </summary>
        Hepatitis_B = 4,
        /// <summary>
        /// Pneumococcal
        /// </summary>
        Pneumococcal = 5,
        /// <summary>
        /// Tetanus Diphtheria
        /// </summary>
        Tetanus_Diphtheria = 6,
        /// <summary>
        /// Zostavax (Shingelles)
        /// </summary>
        Shingelles = 7,
        /// <summary>
        /// Glucose
        /// </summary>
        Glucose = 8,
        /// <summary>
        /// HIV
        /// </summary>
        HIV = 9,
        /// <summary>
        /// Lipid Panel
        /// </summary>
        Lipid_Panel = 10,
        /// <summary>
        /// Prostate Specific Antigen (PSA)
        /// </summary>
        PSA = 11,
        /// <summary>
        /// Abdominal Aortic Aneurysm Sonogram
        /// </summary>
        Abdominal_Aortic_Aneurysm_Sonogram = 12,
        /// <summary>
        /// Bone Density (Dexa) Scan
        /// </summary>
        Dexa_Scan = 13,
        /// <summary>
        /// Colonoscopy
        /// </summary>
        Colonoscopy = 14,
        /// <summary>
        /// Echocardiogram
        /// </summary>
        Echocardiogram = 15,
        /// <summary>
        /// Electrocardiogram (EKG)G0403
        /// </summary>
        Electrocardiogram = 16,
        /// <summary>
        /// Hearing Screening
        /// </summary>
        Hearing_Screening = 17,
        /// <summary>
        /// Hemocult
        /// </summary>
        Hemocult = 18,
        /// <summary>
        /// Mammogram (Breast Cancer) Screen
        /// </summary>
        Mammogram_Screen = 19,
        /// <summary>
        /// Cervical Cancer (Pap/Pelvic) Screening
        /// </summary>
        Cervical_Cancer_Screening = 20,
        /// <summary>
        /// Diabetes Self-Management Training
        /// </summary>
        Diabetes_Training = 21,
        /// <summary>
        /// Glaucoma Screening
        /// </summary>
        Glaucoma_Screening = 22,
        /// <summary>
        /// Nutritional (Dietary Counseling)
        /// </summary>
        Nutritional = 23,
        /// <summary>
        /// Obesity Counseling
        /// </summary>
        Obesity_Counseling = 24,
        /// <summary>
        /// Rectal Exam
        /// </summary>
        Rectal_Exam = 25,
        /// <summary>
        /// Tobacco Cessation
        /// </summary>
        Tobacco_Cessation = 26,
        /// <summary>
        /// HbA1C
        /// </summary>
        HbA1C = 27,
        /// <summary>
        /// LDL
        /// </summary>
        LDL = 28,
    }
}
