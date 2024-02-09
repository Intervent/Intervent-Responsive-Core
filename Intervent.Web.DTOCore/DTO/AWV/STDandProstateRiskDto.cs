namespace Intervent.Web.DTO.AWV
{
    /// <summary>
    /// STD Risk
    /// </summary>
    public class STDandProstateRiskDto
    {
        /// <summary>
        /// Current: Sexually active?
        /// 1. Yes 2. No
        /// </summary>
        public byte? STD_Active { get; set; }

        /// <summary>
        /// Current: with multiple partners?
        /// 1. Yes 2. No
        /// </summary>
        public byte? STD_MulPart { get; set; }

        /// <summary>
        /// Current: With 
        /// 1. Men 2. Women 3. Both
        /// </summary>
        public byte? STD_With { get; set; }

        /// <summary>
        /// Current: using any street drugs?
        /// 1. Yes 2. No
        /// </summary>
        public byte? STD_UsingDrugs { get; set; }

        /// <summary>
        /// use condoms?
        /// 1. Yes 2. No
        /// </summary>
        public byte? STD_UseCondoms { get; set; }

        /// <summary>
        /// have a tattoo?
        /// 1. Yes 2. No
        /// </summary>
        public byte? STD_HaveTattoo { get; set; }

        /// <summary>
        /// have body piercing (other than your ears)?
        /// 1. Yes 2. No
        /// </summary>
        public byte? STD_BodyPierce { get; set; }

        /// <summary>
        /// have you lived in the same house with someone infected with HIV or Hepatitis
        /// 1. Yes 2. No
        /// </summary>
        public byte? STD_LivedWithHIV { get; set; }

        /// <summary>
        /// ever: been in jail or prison?
        /// 1. Yes 2. No
        /// </summary>
        public byte? STD_InJail { get; set; }

        /// <summary>
        /// ever: had a blood transfusion before 1992?
        /// 1. Yes 2. No
        /// </summary>
        public byte? STD_BloodTrans { get; set; }

        /// <summary>
        /// ever: been tested for HIV?
        /// 1. Yes 2. No
        /// </summary>
        public byte? STD_TestedforHIV { get; set; }

        /// <summary>
        /// ever: had sex with someone with a STD?
        /// 1. Yes 2. No
        /// </summary>
        public byte? STD_SexWithSTD { get; set; }

        /// <summary>
        /// ever: had a sexually transmitted disease?
        /// 1. Yes 2. No
        /// </summary>
        public byte? STD_HadSexDis { get; set; }

        /// <summary>
        /// ever: used any street drugs?
        /// 1. Yes 2. No
        /// </summary>
        public byte? STD_UsedDrugs { get; set; }

        /// <summary>
        /// ever: had sexual relations with someone that you did not know their sexual history?
        /// 1. Yes 2. No
        /// </summary>
        public byte? STD_SexWithUnknown { get; set; }

        /// <summary>
        /// ever:received the Hepatitis B Vaccine?
        /// 1. Yes 2. No
        /// </summary>
        public byte? STD_HepBVaccine { get; set; }

        /// <summary>
        /// STD Risk Score
        /// 1. No further evaluation required 2. Possibly at high rish (>3) for STD 3. Advised patient to have HIV test 4. Referred back to PCP
        /// </summary>
        public byte? STDRiskScreen { get; set; }

        /// <summary>
        /// Over the past month how often have you had a sensation of not emptying your bladder completely after you finished urinating?
        /// 1. None 2. Less than 1-5 times 3. less than 1/2 time 4. About 1/2 time 5. More than 1/2 time 6. Always
        /// </summary>
        public byte? Pro_NotEmpty { get; set; }

        /// <summary>
        /// ver the past month, how often have you had to urinate again less than two hours after you finished urinating?
        /// 1. None 2. Less than 1-5 times 3. less than 1/2 time 4. About 1/2 time 5. More than 1/2 time 6. Always
        /// </summary>
        public byte? Pro_AgaininTwo { get; set; }

        /// <summary>
        /// Over the past month, how often have you stopped and started again several times when you urinated?
        /// 1. None 2. Less than 1-5 times 3. less than 1/2 time 4. About 1/2 time 5. More than 1/2 time 6. Always
        /// </summary>
        public byte? Pro_StartandStop { get; set; }

        /// <summary>
        /// Over the past month, how often have you found it difficult to postpone urination?
        /// 1. None 2. Less than 1-5 times 3. less than 1/2 time 4. About 1/2 time 5. More than 1/2 time 6. Always
        /// </summary>
        public byte? Pro_DifftoPost { get; set; }

        /// <summary>
        /// Over the past month, how often have you had a weak urinary stream?
        /// 1. None 2. Less than 1-5 times 3. less than 1/2 time 4. About 1/2 time 5. More than 1/2 time 6. Always
        /// </summary>
        public byte? Pro_WeakSystem { get; set; }

        /// <summary>
        /// Over the past month, how often have you had to push or strain to begin urination?
        /// 1. None 2. Less than 1-5 times 3. less than 1/2 time 4. About 1/2 time 5. More than 1/2 time 6. Always
        /// </summary>
        public byte? Pro_StraintoBegin { get; set; }

        /// <summary>
        /// Over the past month, typically, how many times did you get up to urinate during the night?
        /// 1. None 2. Less than 1-5 times 3. less than 1/2 time 4. About 1/2 time 5. More than 1/2 time 6. Always
        /// </summary>
        public byte? Pro_GetupatNight { get; set; }

        /// <summary>
        /// Prostate Disorder Screen Risk Score
        /// 1. No further evaluation needed 2. Refer to PCP for Mild BPH symptoms 3. Refer to PCP for Moderate BPH symptoms 4. Refer to PCP for Severe BPH symptoms
        /// </summary>
        public byte? ProsRiskScreen { get; set; }

        public Dictionary<string, string> Validate(Dictionary<string, string> validateResult)
        {
            ValidationUtility.Validate(STD_Active, 1, 2, "STD_Active", validateResult);
            ValidationUtility.Validate(STD_MulPart, 1, 2, "STD_MulPart", validateResult);
            ValidationUtility.Validate(STD_With, 1, 3, "STD_With", validateResult);
            ValidationUtility.Validate(STD_UsingDrugs, 1, 2, "STD_UsingDrugs", validateResult);
            ValidationUtility.Validate(STD_UseCondoms, 1, 2, "STD_UseCondoms", validateResult);
            ValidationUtility.Validate(STD_HaveTattoo, 1, 2, "STD_HaveTattoo", validateResult);
            ValidationUtility.Validate(STD_BodyPierce, 1, 2, "STD_BodyPierce", validateResult);
            ValidationUtility.Validate(STD_LivedWithHIV, 1, 2, "STD_LivedWithHIV", validateResult);
            ValidationUtility.Validate(STD_InJail, 1, 2, "STD_InJail", validateResult);
            ValidationUtility.Validate(STD_BloodTrans, 1, 2, "STD_BloodTrans", validateResult);
            ValidationUtility.Validate(STD_TestedforHIV, 1, 2, "STD_TestedforHIV", validateResult);
            ValidationUtility.Validate(STD_SexWithSTD, 1, 2, "STD_SexWithSTD", validateResult);
            ValidationUtility.Validate(STD_HadSexDis, 1, 2, "STD_HadSexDis", validateResult);
            ValidationUtility.Validate(STD_UsedDrugs, 1, 2, "STD_UsedDrugs", validateResult);
            ValidationUtility.Validate(STD_SexWithUnknown, 1, 2, "STD_SexWithUnknown", validateResult);
            ValidationUtility.Validate(STD_HepBVaccine, 1, 2, "STD_HepBVaccine", validateResult);
            ValidationUtility.Validate(STDRiskScreen, 1, 4, "STDRiskScreen", validateResult);
            ValidationUtility.Validate(Pro_NotEmpty, 1, 6, "Pro_NotEmpty", validateResult);
            ValidationUtility.Validate(Pro_AgaininTwo, 1, 6, "Pro_AgaininTwo", validateResult);
            ValidationUtility.Validate(Pro_StartandStop, 1, 6, "Pro_StartandStop", validateResult);
            ValidationUtility.Validate(Pro_DifftoPost, 1, 6, "Pro_DifftoPost", validateResult);
            ValidationUtility.Validate(Pro_WeakSystem, 1, 6, "Pro_WeakSystem", validateResult);
            ValidationUtility.Validate(Pro_StraintoBegin, 1, 6, "Pro_StraintoBegin", validateResult);
            ValidationUtility.Validate(Pro_GetupatNight, 1, 6, "Pro_GetupatNight", validateResult);
            ValidationUtility.Validate(ProsRiskScreen, 1, 4, "ProsRiskScreen", validateResult);

            return validateResult;
        }

    }
}
