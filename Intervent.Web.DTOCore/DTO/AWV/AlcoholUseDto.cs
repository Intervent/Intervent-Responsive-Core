namespace Intervent.Web.DTO.AWV
{
    /// <summary>
    /// Audit: Alcohol Misuse
    /// </summary>
    public class AlcoholUseDto
    {
        /// <summary>
        /// How often do you have a drink containing alcohol?
        /// 1. Never 2. Monthly or less  3. 2 to 4 times a month 4. 2 to 3 times a week 5. 4 or more times a week
        /// </summary>
        public byte? HowOften { get; set; }

        /// <summary>
        /// How many drinks containing alcohol do you have on a typical day when you are drinking?
        /// 1. 1 or 2 2. 3 to 4 3. 5 or 6 4. 7,8 or 9 5. 10 or more
        /// </summary>
        public byte? HowMany { get; set; }

        /// <summary>
        /// How often do you have six or more drinks on one occasion?
        /// 1. Never 2. Less than Monthly 3. Montly 4. Weekly 5. Daily or almost daily
        /// </summary>
        public byte? SixorMore { get; set; }

        /// <summary>
        /// How often during the last year have you found that you were not able to stop drinking once you had started?
        /// 1. Never 2. Less than Monthly 3. Montly 4. Weekly 5. Daily or almost daily
        /// </summary>
        public byte? CantStop { get; set; }

        /// <summary>
        /// How often during the last year have you failed to do what was normally expected from you because of drinking?
        /// 1. Never 2. Less than Monthly 3. Montly 4. Weekly 5. Daily or almost daily
        /// </summary>
        public byte? FailtoDo { get; set; }

        /// <summary>
        /// How often during last year have you needed a first drink in the morning to get yourself going after a heavy drinking session?
        /// 1. Never 2. Less than Monthly 3. Montly 4. Weekly 5. Daily or almost daily
        /// </summary>
        public byte? MorDrink { get; set; }

        /// <summary>
        /// How often during last year have you had a feeling of guilt or remorse after drinking?
        /// 1. Never 2. Less than Monthly 3. Montly 4. Weekly 5. Daily or almost daily
        /// </summary>
        public byte? FeelGuilt { get; set; }

        /// <summary>
        /// How often during last year have you been unable to remember what happend the night before because you had been drinking?
        /// 1. Never 2. Less than Monthly 3. Montly 4. Weekly 5. Daily or almost daily
        /// </summary>
        public byte? DontRem { get; set; }

        /// <summary>
        /// Have you or someone else been injured as a result of your drinking?
        ///  1. No 2. Yes, but not in last year 3. Yes, during last year
        /// </summary>
        public byte? Injured { get; set; }

        /// <summary>
        ///  Has a relative or friend or a doctor or other health worker been concerned about your drinking or suggested you cut down?
        ///  1. No 2. Yes, but not in last year 3. Yes, during last year
        /// </summary>
        public byte? Concerned { get; set; }

        /// <summary>
        /// Provider Assessment
        /// 1. No further Evaluation required 2. Refer PCP due to hazardous use 3. Refer PCP due to alcohol dependence
        /// </summary>
        public byte? AlcoholScore { get; set; }

        public Dictionary<string, string> Validate(Dictionary<string, string> validateResult)
        {
            ValidationUtility.Validate(HowOften, 1, 5, "HowOften", validateResult);
            ValidationUtility.Validate(HowMany, 1, 5, "HowMany", validateResult);
            ValidationUtility.Validate(SixorMore, 1, 5, "SixorMore", validateResult);
            ValidationUtility.Validate(CantStop, 1, 5, "CantStop", validateResult);
            ValidationUtility.Validate(FailtoDo, 1, 5, "FailtoDo", validateResult);
            ValidationUtility.Validate(MorDrink, 1, 5, "MorDrink", validateResult);
            ValidationUtility.Validate(FeelGuilt, 1, 5, "FeelGuilt", validateResult);
            ValidationUtility.Validate(DontRem, 1, 5, "DontRem", validateResult);
            ValidationUtility.Validate(Injured, 1, 3, "Injured", validateResult);
            ValidationUtility.Validate(Concerned, 1, 3, "Concerned", validateResult);
            ValidationUtility.Validate(AlcoholScore, 1, 3, "AlcoholScore", validateResult);
            return validateResult;
        }
    }
}
