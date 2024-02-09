namespace Intervent.Web.DTO.AWV
{
    /// <summary>
    /// Medical History and Providers
    /// </summary>
    public class MedicalHistandProvidersDto
    {
        /// <summary>
        /// Family History: Anemia
        /// 1. Yes 2. No
        /// </summary>
        public byte? Adopted { get; set; }

        /// <summary>
        /// List of Other Chronic Problems and Surgeries        
        /// </summary>
        public List<string> OtherChronProbandSurg { get; set; }

        /// <summary>
        /// Comments
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Medical Conditions
        /// </summary>
        public MedicalConditionsDto MedicalConditions { get; set; }

        /// <summary>
        /// Surgeries
        /// </summary>
        public SurgeriesDto Surgeries { get; set; }

        /// <summary>
        /// Family Medical History 
        /// </summary>
        public FamilyHistoryDto FamilyHistory { get; set; }

        /// <summary>
        /// Hospital/Urgent Care Visits
        /// </summary>
        public HospitalVisitsDto HospitalVisit { get; set; }

        /// <summary>
        /// Medications
        /// </summary>
        public MedicationsDto Medications { get; set; }

        /// <summary>
        /// Specialities, CareGivers and Health Care Services
        /// </summary>
        public ServicesDto Services { get; set; }

        /// <summary>
        /// Drug Allergies
        /// </summary>
        public AllergiesDto Allergies { get; set; }

        public Dictionary<string, string> Validate(Dictionary<string, string> validateResult)
        {
            ValidationUtility.Validate(Adopted, 1, 2, "Adopted", validateResult);
            if (MedicalConditions != null)
                validateResult = MedicalConditions.Validate(validateResult);
            if (Surgeries != null)
                validateResult = Surgeries.Validate(validateResult);
            if (FamilyHistory != null)
                validateResult = FamilyHistory.Validate(validateResult);
            return validateResult;
        }
    }

    #region Medical Conditions
    /// <summary>
    /// Medical History and Providers  - Medical Conditions
    /// </summary>
    public class MedicalConditionsDto
    {
        /// <summary>
        /// Medical Condition: Alcoholism
        /// 1. Yes
        /// </summary>
        public byte? Alcoholism { get; set; }

        /// <summary>
        /// Medical Condition: Anemia
        /// 1. Yes
        /// </summary>
        public byte? Anemia { get; set; }

        /// <summary>
        /// Medical Condition: Asthma
        /// 1. Yes
        /// </summary>
        public byte? Asthma { get; set; }

        /// <summary>
        /// Medical Condition: Arthritis
        /// 1. Yes
        /// </summary>
        public byte? Arthritis { get; set; }

        /// <summary>
        /// Medical Condition: Bladder Problems
        /// 1. Yes
        /// </summary>
        public byte? BladderProblems { get; set; }

        /// <summary>
        /// Medical Condition: Blindness
        /// 1. Yes
        /// </summary>
        public byte? Blindness { get; set; }

        /// <summary>
        /// Medical Condition: Blood Disorder
        /// 1. Yes
        /// </summary>
        public byte? BloodDisorder { get; set; }

        /// <summary>
        /// Medical Condition: Blood Clots
        /// 1. Yes
        /// </summary>
        public byte? BloodClots { get; set; }

        /// <summary>
        /// Medical Condition: Breast Cancer
        /// 1. Yes
        /// </summary>
        public byte? BreastCancer { get; set; }

        /// <summary>
        /// Medical Condition: Colon Cancer
        /// 1. Yes
        /// </summary>
        public byte? ColonCancer { get; set; }

        /// <summary>
        /// Medical Condition: Colon Polyps
        /// 1. Yes
        /// </summary>
        public byte? ColonPolyps { get; set; }

        /// <summary>
        /// Medical Condition: Cancer Other
        /// 1. Yes
        /// </summary>
        public byte? CancerOther { get; set; }

        /// <summary>
        /// Medical Condition: Cataracts
        /// 1. Yes
        /// </summary>
        public byte? Cataracts { get; set; }

        /// <summary>
        /// Medical Condition: Depression
        /// 1. Yes
        /// </summary>
        public byte? Depression { get; set; }

        /// <summary>
        /// Medical Condition: Diabetes
        /// 1. Yes
        /// </summary>
        public byte? Diabetes { get; set; }

        /// <summary>
        /// Medical Condition: Emphysema (COPD)
        /// 1. Yes
        /// </summary>
        public byte? COPD { get; set; }

        /// <summary>
        /// Medical Condition: Gastric Reflex (GERD)
        /// 1. Yes
        /// </summary>
        public byte? GERD { get; set; }

        /// <summary>
        /// Medical Condition: Glaucoma
        /// 1. Yes
        /// </summary>
        public byte? Glaucoma { get; set; }

        /// <summary>
        /// Medical Condition: Hay Fever
        /// 1. Yes
        /// </summary>
        public byte? HayFever { get; set; }

        /// <summary>
        /// Medical Condition: Hearing Loss
        /// 1. Yes
        /// </summary>
        public byte? HearingLoss { get; set; }

        /// <summary>
        /// Medical Condition: Heart Attack
        /// 1. Yes
        /// </summary>
        public byte? HeartAttack { get; set; }

        /// <summary>
        /// Medical Condition: Heart Disease
        /// 1. Yes
        /// </summary>
        public byte? HeartDisease { get; set; }

        /// <summary>
        /// Medical Condition: Hepatitis
        /// 1. Yes
        /// </summary>
        public byte? Hepatitis { get; set; }

        /// <summary>
        /// Medical Condition: High Cholesterol
        /// 1. Yes
        /// </summary>
        public byte? HighCholesterol { get; set; }

        /// <summary>
        /// Medical Condition: Anemia
        /// 1. Yes
        /// </summary>
        public byte? Jaundice { get; set; }

        /// <summary>
        /// Medical Condition: Gout
        /// 1. Yes
        /// </summary>
        public byte? Gout { get; set; }

        /// <summary>
        /// Medical Condition: Hypertension (High BP)
        /// 1. Yes
        /// </summary>
        public byte? Hypertension { get; set; }

        /// <summary>
        /// Medical Condition: HIV or AIDS
        /// 1. Yes
        /// </summary>
        public byte? HIVorAIDS { get; set; }

        /// <summary>
        /// Medical Condition: Kidney Disease
        /// 1. Yes
        /// </summary>
        public byte? KidneyDisease { get; set; }

        /// <summary>
        /// Medical Condition: Kidney Stones
        /// 1. Yes
        /// </summary>
        public byte? KidneyStones { get; set; }

        /// <summary>
        /// Medical Condition: Liver Disease
        /// 1. Yes
        /// </summary>
        public byte? LiverDisease { get; set; }

        /// <summary>
        /// Medical Condition: Mental Illness
        /// 1. Yes
        /// </summary>
        public byte? MentalIllness { get; set; }

        /// <summary>
        /// Medical Condition: Neurologic Disease
        /// 1. Yes
        /// </summary>
        public byte? NeurologicDisease { get; set; }

        /// <summary>
        /// Medical Condition: Osteoporosis
        /// 1. Yes
        /// </summary>
        public byte? Osteoporosis { get; set; }

        /// <summary>
        /// Medical Condition: Pacemaker
        /// 1. Yes
        /// </summary>
        public byte? Pacemaker { get; set; }

        /// <summary>
        /// Medical Condition: Physical Disability
        /// 1. Yes
        /// </summary>
        public byte? PhysicalDisability { get; set; }

        /// <summary>
        /// Medical Condition: Pneumonia
        /// 1. Yes
        /// </summary>
        public byte? Pneumonia { get; set; }

        /// <summary>
        /// Medical Condition: Rheumatic Fever
        /// 1. Yes
        /// </summary>
        public byte? RheumaticFever { get; set; }

        /// <summary>
        /// Medical Condition: Seizure Disorder
        /// 1. Yes
        /// </summary>
        public byte? SeizureDisorder { get; set; }

        /// <summary>
        /// Medical Condition: Sleep Disorder
        /// 1. Yes
        /// </summary>
        public byte? SleepDisorder { get; set; }

        /// <summary>
        /// Medical Condition: Stomach Disorder
        /// 1. Yes
        /// </summary>
        public byte? StomachDisorder { get; set; }

        /// <summary>
        /// Medical Condition: Stroke (CVA)
        /// 1. Yes
        /// </summary>
        public byte? Stroke { get; set; }

        /// <summary>
        /// Medical Condition: Thyroid Disease
        /// 1. Yes
        /// </summary>
        public byte? ThyroidDisease { get; set; }

        /// <summary>
        /// Medical Condition: Tuberculosis (TB)
        /// 1. Yes
        /// </summary>
        public byte? Tuberculosis { get; set; }

        /// <summary>
        /// Medical Condition: Ulcer
        /// 1. Yes
        /// </summary>
        public byte? Ulcer { get; set; }

        public Dictionary<string, string> Validate(Dictionary<string, string> validateResult)
        {
            ValidationUtility.Validate(Alcoholism, 1, 1, "Alcoholism", validateResult);
            ValidationUtility.Validate(Anemia, 1, 1, "Anemia", validateResult);
            ValidationUtility.Validate(Asthma, 1, 1, "Asthma", validateResult);
            ValidationUtility.Validate(Arthritis, 1, 1, "Arthritis", validateResult);
            ValidationUtility.Validate(BladderProblems, 1, 1, "BladderProblems", validateResult);
            ValidationUtility.Validate(Blindness, 1, 1, "Blindness", validateResult);
            ValidationUtility.Validate(BloodClots, 1, 1, "BloodClots", validateResult);
            ValidationUtility.Validate(BloodDisorder, 1, 1, "BloodDisorder", validateResult);
            ValidationUtility.Validate(BreastCancer, 1, 1, "BreastCancer", validateResult);
            ValidationUtility.Validate(ColonCancer, 1, 1, "ColonCancer", validateResult);
            ValidationUtility.Validate(ColonPolyps, 1, 1, "ColonPolyps", validateResult);
            ValidationUtility.Validate(CancerOther, 1, 1, "CancerOther", validateResult);
            ValidationUtility.Validate(Cataracts, 1, 1, "Cataracts", validateResult);
            ValidationUtility.Validate(Depression, 1, 1, "Depression", validateResult);
            ValidationUtility.Validate(Diabetes, 1, 1, "Diabetes", validateResult);
            ValidationUtility.Validate(COPD, 1, 1, "COPD", validateResult);
            ValidationUtility.Validate(GERD, 1, 1, "GERD", validateResult);
            ValidationUtility.Validate(Glaucoma, 1, 1, "Glaucoma", validateResult);
            ValidationUtility.Validate(HayFever, 1, 1, "HayFever", validateResult);
            ValidationUtility.Validate(HeartAttack, 1, 1, "HeartAttack", validateResult);
            ValidationUtility.Validate(HearingLoss, 1, 1, "HearingLoss", validateResult);
            ValidationUtility.Validate(HeartDisease, 1, 1, "HeartDisease", validateResult);
            ValidationUtility.Validate(Hepatitis, 1, 1, "Hepatitis", validateResult);
            ValidationUtility.Validate(HighCholesterol, 1, 1, "HighCholesterol", validateResult);
            ValidationUtility.Validate(Jaundice, 1, 1, "Jaundice", validateResult);
            ValidationUtility.Validate(Gout, 1, 1, "Gout", validateResult);
            ValidationUtility.Validate(Hypertension, 1, 1, "Hypertension", validateResult);
            ValidationUtility.Validate(HIVorAIDS, 1, 1, "HIVorAIDS", validateResult);
            ValidationUtility.Validate(KidneyDisease, 1, 1, "KidneyDisease", validateResult);
            ValidationUtility.Validate(KidneyStones, 1, 1, "KidneyStones", validateResult);
            ValidationUtility.Validate(LiverDisease, 1, 1, "LiverDisease", validateResult);
            ValidationUtility.Validate(MentalIllness, 1, 1, "MentalIllness", validateResult);
            ValidationUtility.Validate(NeurologicDisease, 1, 1, "NeurologicDisease", validateResult);
            ValidationUtility.Validate(Osteoporosis, 1, 1, "MedicalConditions.Osteoporosis", validateResult);
            ValidationUtility.Validate(Pacemaker, 1, 1, "Pacemaker", validateResult);
            ValidationUtility.Validate(PhysicalDisability, 1, 1, "PhysicalDisability", validateResult);
            ValidationUtility.Validate(Pneumonia, 1, 1, "Pneumonia", validateResult);
            ValidationUtility.Validate(RheumaticFever, 1, 1, "RheumaticFever", validateResult);
            ValidationUtility.Validate(SeizureDisorder, 1, 1, "SeizureDisorder", validateResult);
            ValidationUtility.Validate(SleepDisorder, 1, 1, "SleepDisorder", validateResult);
            ValidationUtility.Validate(StomachDisorder, 1, 1, "StomachDisorder", validateResult);
            ValidationUtility.Validate(Stroke, 1, 1, "Stroke", validateResult);
            ValidationUtility.Validate(ThyroidDisease, 1, 1, "ThyroidDisease", validateResult);
            ValidationUtility.Validate(Tuberculosis, 1, 1, "Tuberculosis", validateResult);
            ValidationUtility.Validate(Ulcer, 1, 1, "Ulcer", validateResult);
            return validateResult;
        }
    }
    #endregion

    #region Surgeries
    /// <summary>
    /// Medical History And Providers : Surgeries
    /// </summary>
    public class SurgeriesDto
    {
        /// <summary>
        /// Surgeries: Abdominal
        /// 1. Yes
        /// </summary>
        public byte? Abdominal { get; set; }

        /// <summary>
        /// Surgeries: Appendectomy
        /// 1. Yes
        /// </summary>
        public byte? Appendectomy { get; set; }

        /// <summary>
        /// Surgeries: Back Surgery
        /// 1. Yes
        /// </summary>
        public byte? BackSurgery { get; set; }

        /// <summary>
        /// Surgeries: Breast Surgery
        /// 1. Yes
        /// </summary>
        public byte? BreastSurgery { get; set; }

        /// <summary>
        /// Surgeries: C-Section
        /// 1. Yes
        /// </summary>
        public byte? CSection { get; set; }

        /// <summary>
        /// Surgeries: Colonoscopy
        /// 1. Yes
        /// </summary>
        public byte? Colonoscopy { get; set; }

        /// <summary>
        /// Surgeries: D and C
        /// 1. Yes
        /// </summary>
        public byte? DandC { get; set; }

        /// <summary>
        /// Surgeries: EGD (Stomach scope)
        /// 1. Yes
        /// </summary>
        public byte? EGD { get; set; }

        /// <summary>
        /// Surgeries: Eye Surgery
        /// 1. Yes
        /// </summary>
        public byte? EyeSurgery { get; set; }

        /// <summary>
        /// Surgeries: Gallbladder
        /// 1. Yes
        /// </summary>
        public byte? Gallbladder { get; set; }

        /// <summary>
        /// Surgeries: Heart Cath
        /// 1. Yes
        /// </summary>
        public byte? HeartCath { get; set; }

        /// <summary>
        /// Surgeries: Heart Surgery
        /// 1. Yes
        /// </summary>
        public byte? HeartSurgery { get; set; }

        /// <summary>
        /// Surgeries: Hernia Repair
        /// 1. Yes
        /// </summary>
        public byte? HerniaRepair { get; set; }

        /// <summary>
        /// Surgeries: Hip Surgery
        /// 1. Yes
        /// </summary>
        public byte? HipSurgery { get; set; }

        /// <summary>
        /// Surgeries: Hysterectomy
        /// 1. Yes
        /// </summary>
        public byte? Hysterectomy { get; set; }

        /// <summary>
        /// Surgeries: Knee Surgery
        /// 1. Yes
        /// </summary>
        public byte? KneeSurgery { get; set; }

        /// <summary>
        /// Surgeries: Nasal Surgery
        /// 1. Yes
        /// </summary>
        public byte? NasalSurgery { get; set; }

        /// <summary>
        /// Surgeries: Neck Surgery
        /// 1. Yes
        /// </summary>
        public byte? NeckSurgery { get; set; }

        /// <summary>
        /// Surgeries: Sinus Surgery
        /// 1. Yes
        /// </summary>
        public byte? SinusSurgery { get; set; }

        /// <summary>
        /// Surgeries: Ovarian Surgery
        /// 1. Yes
        /// </summary>
        public byte? OvarianSurgery { get; set; }

        /// <summary>
        /// Surgeries: Plastic Surgery
        /// 1. Yes
        /// </summary>
        public byte? PlasticSurgery { get; set; }

        /// <summary>
        /// Surgeries: Thyroidectomy
        /// 1. Yes
        /// </summary>
        public byte? Thyroidectomy { get; set; }

        /// <summary>
        /// Surgeries: Tonsil and Adenoid
        /// 1. Yes
        /// </summary>
        public byte? TonsilandAdenoid { get; set; }

        /// <summary>
        /// Surgeries: Tubal Ligation
        /// 1. Yes
        /// </summary>
        public byte? TubalLigation { get; set; }

        /// <summary>
        /// Surgeries: Vasectomy
        /// 1. Yes
        /// </summary>
        public byte? Vasectomy { get; set; }

        /// <summary>
        /// Surgeries: Prostate Surgery
        /// 1. Yes
        /// </summary>
        public byte? ProstateSurgery { get; set; }

        public Dictionary<string, string> Validate(Dictionary<string, string> validateionResult)
        {
            ValidationUtility.Validate(Abdominal, 1, 1, "Abdominal", validateionResult);
            ValidationUtility.Validate(Appendectomy, 1, 1, "Appendectomy", validateionResult);
            ValidationUtility.Validate(BackSurgery, 1, 1, "BackSurgery", validateionResult);
            ValidationUtility.Validate(BreastSurgery, 1, 1, "BreastSurgery", validateionResult);
            ValidationUtility.Validate(CSection, 1, 1, "CSection", validateionResult);
            ValidationUtility.Validate(Colonoscopy, 1, 1, "Colonoscopy", validateionResult);
            ValidationUtility.Validate(DandC, 1, 1, "DandC", validateionResult);
            ValidationUtility.Validate(EGD, 1, 1, "EGD", validateionResult);
            ValidationUtility.Validate(EyeSurgery, 1, 1, "EyeSurgery", validateionResult);
            ValidationUtility.Validate(Gallbladder, 1, 1, "Gallbladder", validateionResult);
            ValidationUtility.Validate(HeartCath, 1, 1, "HeartCath", validateionResult);
            ValidationUtility.Validate(HeartSurgery, 1, 1, "HeartSurgery", validateionResult);
            ValidationUtility.Validate(HerniaRepair, 1, 1, "HerniaRepair", validateionResult);
            ValidationUtility.Validate(HipSurgery, 1, 1, "HipSurgery", validateionResult);
            ValidationUtility.Validate(Hysterectomy, 1, 1, "Hysterectomy", validateionResult);
            ValidationUtility.Validate(KneeSurgery, 1, 1, "KneeSurgery", validateionResult);
            ValidationUtility.Validate(NasalSurgery, 1, 1, "NasalSurgery", validateionResult);
            ValidationUtility.Validate(NeckSurgery, 1, 1, "NeckSurgery", validateionResult);
            ValidationUtility.Validate(SinusSurgery, 1, 1, "SinusSurgery", validateionResult);
            ValidationUtility.Validate(OvarianSurgery, 1, 1, "OvarianSurgery", validateionResult);
            ValidationUtility.Validate(PlasticSurgery, 1, 1, "PlasticSurgery", validateionResult);
            ValidationUtility.Validate(Thyroidectomy, 1, 1, "Thyroidectomy", validateionResult);
            ValidationUtility.Validate(TonsilandAdenoid, 1, 1, "TonsilandAdenoid", validateionResult);
            ValidationUtility.Validate(TubalLigation, 1, 1, "TubalLigation", validateionResult);
            ValidationUtility.Validate(Vasectomy, 1, 1, "Vasectomy", validateionResult);
            ValidationUtility.Validate(ProstateSurgery, 1, 1, "ProstateSurgery", validateionResult);

            return validateionResult;
        }

    }
    #endregion

    #region Family History
    /// <summary>
    /// Family Medical History 
    /// </summary>
    public class FamilyHistoryDto
    {
        /// <summary>
        /// Family History: Alcohol abuse
        /// 1. Yes 2. No
        /// </summary>
        public byte? FHAlcohol { get; set; }

        /// <summary>
        /// Family History: Other Substance Abuse
        /// 1. Yes 2. No
        /// </summary>
        public byte? FHSubsAbuse { get; set; }

        /// <summary>
        /// Family History: Alzheimers
        /// 1. Yes 2. No
        /// </summary>
        public byte? FHAlzheimers { get; set; }

        /// <summary>
        /// Family History: Dementia
        /// 1. Yes 2. No
        /// </summary>
        public byte? FHDementia { get; set; }

        /// <summary>
        /// Family History: Breast Cancer
        /// 1. Yes 2. No
        /// </summary>
        public byte? FHBreastCancer { get; set; }

        /// <summary>
        /// Family History: Prostate Cancer
        /// 1. Yes 2. No
        /// </summary>
        public byte? FHProstateCancer { get; set; }

        /// <summary>
        /// Family History: Cancer (Other)
        /// 1. Yes 2. No
        /// </summary>
        public byte? FHOtherCancer { get; set; }

        /// <summary>-
        /// Family History: Diabetes
        /// 1. Yes 2. No
        /// </summary>
        public byte? FHDiabetes { get; set; }

        /// <summary>
        /// Family History: Emotional or Mental Illness
        /// 1. Yes 2. No
        /// </summary>
        public byte? FHMentalIllness { get; set; }

        /// <summary>
        /// Family History: Suicide
        /// 1. Yes 2. No
        /// </summary>
        public byte? FHSuicide { get; set; }

        /// <summary>
        /// Family History: Hypertension
        /// 1. Yes 2. No
        /// </summary>
        public byte? FHHypertension { get; set; }

        /// <summary>
        /// Family History: Heart Attack
        /// 1. Yes 2. No
        /// </summary>
        public byte? FHHeartAttack { get; set; }

        /// <summary>
        /// Family History: Osteoporosis
        /// 1. Yes 2. No
        /// </summary>
        public byte? FHOsteoporosis { get; set; }

        /// <summary>
        /// Family History: Seizures
        /// 1. Yes 2. No
        /// </summary>
        public byte? FHSeizures { get; set; }

        /// <summary>
        /// Family History: Stroke
        /// 1. Yes 2. No
        /// </summary>
        public byte? FHStroke { get; set; }

        /// <summary>
        /// Family History: Tuberculosis (TB)
        /// 1. Yes 2. No
        /// </summary>
        public byte? FHTB { get; set; }

        /// <summary>
        /// Family History: Tuberculosis (TB)
        /// 1. Yes 2. No
        /// </summary>
        public byte? FHColonCancer { get; set; }

        public Dictionary<string, string> Validate(Dictionary<string, string> validationResults)
        {
            ValidationUtility.Validate(FHAlcohol, 1, 2, "FHAlcohol", validationResults);
            ValidationUtility.Validate(FHSubsAbuse, 1, 2, "FHSubsAbuse", validationResults);
            ValidationUtility.Validate(FHAlzheimers, 1, 2, "FHAlzheimers", validationResults);
            ValidationUtility.Validate(FHDementia, 1, 2, "FHDementia", validationResults);
            ValidationUtility.Validate(FHBreastCancer, 1, 2, "FHBreastCancer", validationResults);
            ValidationUtility.Validate(FHProstateCancer, 1, 2, "FHProstateCancer", validationResults);
            ValidationUtility.Validate(FHOtherCancer, 1, 2, "FHOtherCancer", validationResults);
            ValidationUtility.Validate(FHDiabetes, 1, 2, "FHDiabetes", validationResults);
            ValidationUtility.Validate(FHMentalIllness, 1, 2, "FHMentalIllness", validationResults);
            ValidationUtility.Validate(FHSuicide, 1, 2, "FHSuicide", validationResults);
            ValidationUtility.Validate(FHHypertension, 1, 2, "FHHypertension", validationResults);
            ValidationUtility.Validate(FHHeartAttack, 1, 2, "FHHeartAttack", validationResults);
            ValidationUtility.Validate(FHOsteoporosis, 1, 2, "FamilyHistory.FHOsteoporosis", validationResults);
            ValidationUtility.Validate(FHSeizures, 1, 2, "FHSeizures", validationResults);
            ValidationUtility.Validate(FHStroke, 1, 2, "FHStroke", validationResults);
            ValidationUtility.Validate(FHTB, 1, 2, "FHTB", validationResults);
            ValidationUtility.Validate(FHColonCancer, 1, 2, "FHColonCancer", validationResults);

            return validationResults;
        }
    }
    #endregion

    #region Hospital Visits
    /// <summary>
    /// Medical History and Providers : Hospital Visits
    /// </summary>
    public class HospitalVisitsDto
    {
        /// <summary>
        /// Hospital Visits: Reason
        /// </summary>        
        public string Description { get; set; }

        /// <summary>
        /// Hospital Visits: Facility
        /// </summary>
        public string Facility { get; set; }

        /// <summary>
        /// Hospital Visits: Physician Name
        /// </summary>
        public string AttendingPhysician { get; set; }

        /// <summary>
        /// Hospital Visits: Date of hospital visit (UTC Format)
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Hospital Visits: Past Surgeries (date + complications)
        /// </summary>
        public string Surgeries { get; set; }
    }
    #endregion

    #region Services
    /// <summary>
    /// Medical History and Providers: Specialities, CareGivers & Health Care Services
    /// </summary>
    public class ServicesDto
    {
        /// <summary>
        /// Service Date (UTC Format)
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Provider Name
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Service Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Type of Service
        /// </summary>
        public string Type { get; set; }
    }
    #endregion  

    #region Medications
    /// <summary>
    /// Medical History and Providers: Medications
    /// </summary>
    public class MedicationsDto
    {
        /// <summary>
        /// Medication Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Dose
        /// </summary>
        public string Dosage { get; set; }

        /// <summary>
        /// Frequency
        /// </summary>
        public string Frequency { get; set; }
    }
    #endregion

    #region Allergies
    /// <summary>
    /// Drug Allergies
    /// </summary>
    public class AllergiesDto
    {
        /// <summary>
        /// Drug Name
        /// </summary>
        public string DrugName { get; set; }

        /// <summary>
        /// Allergic Reaction
        /// </summary>
        public string Allergy { get; set; }

    }
    #endregion
}
