namespace Intervent.Web.DTO.AWV
{
    /// <summary>
    /// Encourage To Quit Tobacco
    /// </summary>
    public class TobaccoUseDto
    {
        /// <summary>
        /// Encourage to Quit Date  (UTC Format)
        /// </summary>
        public DateTime? EnctoQuitDate { get; set; }

        /// <summary>
        /// I have used Cigarettes
        /// 1. Yes
        /// </summary>
        public byte? Cigarettes { get; set; }

        /// <summary>
        /// I have used Smokeless Tobacco
        /// 1. Yes
        /// </summary>
        public byte? SmokelessTob { get; set; }

        /// <summary>
        /// Number of Cigs/Dips/Day
        /// </summary>
        public byte? NoPerDay { get; set; }

        /// <summary>
        /// Number of Years Cigs/Smokeless 
        /// </summary>
        public byte? NoofYears { get; set; }

        /// <summary>
        /// Smoat at Home
        /// 1. Yes
        /// </summary>
        public byte? SmokeAtHome { get; set; }

        /// <summary>
        /// Smoat at Work
        /// 1. Yes
        /// </summary>
        public byte? SmokeAtWork { get; set; }

        /// <summary>
        /// Smoat at Socially
        /// 1. Yes
        /// </summary>
        public byte? SmokeAtSocial { get; set; }

        /// <summary>
        /// Others in the home Smoke
        /// 1. Yes 2. No
        /// </summary>
        public byte? OthersatHome { get; set; }

        /// <summary>
        /// # of time tried quit tobacco use 
        /// </summary>
        public byte? TimesTried { get; set; }

        /// <summary>
        /// Succeeded for how many months?
        /// </summary>
        public short? SuccessFor { get; set; }

        /// <summary>
        /// used in an attempt to quit and has helped.
        /// </summary>
        public List<AidToQuitTobacco> Helped { get; set; }

        /// <summary>
        /// used in an attempt to quit and has helped (Other) 
        /// </summary>
        public string HelpedOthers { get; set; }

        /// <summary>
        /// used in an attempt to quit and I had side effects or failed. 
        /// </summary>
        public List<AidToQuitTobacco> Failed { get; set; }

        /// <summary>
        /// used in an attempt to quit and I had side effects or failed (Other)
        /// </summary>
        public string FailedOthers { get; set; }

        /// <summary>
        /// motivation to quit
        /// 1. Great 2. Good 3. Poor
        /// </summary>
        public byte? Motive { get; set; }

        /// <summary>
        /// Motivation due to 
        /// 1. Health 2. Money 3. Family 4. Work 5. Socially acceptance
        /// </summary>
        public byte? MotiveDueto { get; set; }

        /// <summary>
        /// If I decided to quit those in my home would 
        /// 1. Support 2. Not Supportm2
        /// </summary>
        public byte? FamilyHelp { get; set; }

        /// <summary>
        /// Ready to quit and would like to try
        /// </summary>
        public List<AidToQuitTobacco> UseWhenReady { get; set; }

        /// <summary>
        /// Decrease my use from date (UTC Format)
        /// </summary>
        public DateTime? DecFrom { get; set; }

        /// <summary>
        /// Decrease my use to date (UTC Format)
        /// </summary>
        public DateTime? DecTo { get; set; }

        /// <summary>
        ///  chosen quit date date (UTC Format)
        /// </summary>
        public DateTime? ChosQuitDate { get; set; }

        /// <summary>
        /// Not ready to quit tobacco use
        /// 1. Yes
        /// </summary>
        public byte? NoReady { get; set; }

        public Dictionary<string, string> Validate(Dictionary<string, string> validateResult)
        {
            ValidationUtility.Validate(Cigarettes, 1, 1, "Cigarettes", validateResult);
            ValidationUtility.Validate(SmokelessTob, 1, 1, "SmokelessTob", validateResult);
            ValidationUtility.Validate(SmokeAtWork, 1, 1, "SmokeAtWork", validateResult);
            ValidationUtility.Validate(SmokeAtHome, 1, 1, "SmokeAtHome", validateResult);
            ValidationUtility.Validate(SmokeAtSocial, 1, 1, "SmokeAtSocial", validateResult);
            ValidationUtility.Validate(OthersatHome, 1, 2, "OthersatHome", validateResult);
            ValidationUtility.Validate(Motive, 1, 3, "Motive", validateResult);
            ValidationUtility.Validate(MotiveDueto, 1, 5, "MotiveDueto", validateResult);
            ValidationUtility.Validate(FamilyHelp, 1, 2, "FamilyHelp", validateResult);
            ValidationUtility.Validate(NoReady, 1, 1, "NoReady", validateResult);

            return validateResult;
        }

    }

    public enum AidType
    {
        Helped = 1,
        Failed = 2,
        Ready = 3,
    }

    public enum AidToQuitTobacco
    {
        /// <summary>
        /// Nicotine Patch
        /// </summary>
        Nicotine_Patch = 1,
        /// <summary>
        /// Nicotine Gum
        /// </summary>
        Nicotine_Gum = 2,
        /// <summary>
        /// Nicotine Nasal Spray
        /// </summary>
        Nicotine_Nasal_Spray = 3,
        /// <summary>
        /// Commit
        /// </summary>
        Commit = 4,
        /// <summary>
        /// bupropion (WellButrin)
        /// </summary>
        bupropion = 5,
        /// <summary>
        /// varenicline
        /// </summary>
        varenicline = 6,
        /// <summary>
        /// Quest Cigarettes
        /// </summary>
        Quest_Cigarettes = 7,
        /// <summary>
        /// E-Cig
        /// </summary>
        E_Cig = 8,
        /// <summary>
        /// Acupuncture
        /// </summary>
        Acupuncture = 9,
        /// <summary>
        /// Hypnosis
        /// </summary>
        Hypnosis = 10,
        /// <summary>
        /// Counseling/Support Group
        /// </summary>
        Counseling_SupportGroup = 11,
        /// <summary>
        /// EZ-Smoker
        /// </summary>
        EZ_Smoker = 12,
        /// <summary>
        /// "Cold Turkey"
        /// </summary>
        Cold_Turkey = 13,
        /// <summary>
        /// Exercise
        /// </summary>
        Exercise = 14,
        /// <summary>
        /// Nothing
        /// </summary>
        Nothing = 15,

    }
}
