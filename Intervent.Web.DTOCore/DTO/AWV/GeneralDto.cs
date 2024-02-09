namespace Intervent.Web.DTO.AWV
{
    /// <summary>
    /// Annual Wellness Visit - General Health 
    /// </summary>
    public class GeneralDto
    {
        /// <summary>
        /// How would you rate your health in general
        /// 1 - Good 2 - Normal 3 - Not Good
        /// </summary>
        public byte? HealthRating { get; set; }

        /// <summary>
        ///  Are things getting better or worse for you?
        /// 1 - Good 2 - Normal 3 - Not Good
        /// </summary>
        public byte? HealthStatus { get; set; }

        /// <summary>
        /// Any trouble taking your medications
        /// 1 - Yes 2 - No
        /// </summary>
        public byte? TroubleTakingMeds { get; set; }

        /// <summary>
        /// Dizziness or falling when standing?
        /// 1 - Yes 2 - No
        /// </summary>
        public byte? BotheredbyDizzy { get; set; }

        /// <summary>
        /// Sexual Problems?
        /// 1 - Yes 2 - No
        /// </summary>
        public byte? BotheredbySex { get; set; }

        /// <summary>
        /// Trouble Eating?
        /// 1 - Yes 2 - No
        /// </summary>
        public byte? BotheredbyEating { get; set; }

        /// <summary>
        /// Teeth or Denture Problems?
        /// 1 - Yes 2 - No
        /// </summary>
        public byte? BotheredbyTeeth { get; set; }

        /// <summary>
        /// Problems using telephone?
        /// 1 - Yes 2 - No
        /// </summary>
        public byte? BotheredbyTelephone { get; set; }

        /// <summary>
        /// Tiredness or fatigue?
        /// 1 - Yes 2 - No
        /// </summary>
        public byte? BotheredbyTiredness { get; set; }

        /// <summary>
        /// Do you use hearing aids?
        /// 1 - Yes 2 - No
        /// </summary>
        public byte? UseHearingAids { get; set; }

        /// <summary>
        /// Have you been feeling down, depressed or hopeless?
        /// 1 - Yes 2 - No
        /// </summary>
        public byte? FeelingDown { get; set; }

        /// <summary>
        /// Have you had little interest or pleasure in your usual activities?
        /// 1 - Yes 2 - No
        /// </summary>
        public byte? LittleInterest { get; set; }

        /// <summary>
        /// When you feel unwell or distressed whom do you go to for help?
        /// </summary>
        public string GoToHelp { get; set; }

        /// <summary>
        /// How many days a week do you eat a good breakfast?
        /// 1 - [0] 2 - [1-3] 3 - [4+]
        /// </summary>
        public byte? BreakfastDays { get; set; }

        /// <summary>
        /// How many sweetened drinks do you have a day?
        /// 1 - [0] 2 - [1-2] 3 - [3+]
        /// </summary>
        public byte? NoOfSweetDrinks { get; set; }

        /// <summary>
        /// How many serving of fruits or vegetables do you have a day?
        /// 1 - [0] 2 - [1-3] 3 - [4+]
        /// </summary>
        public byte? FruitandVegServ { get; set; }

        /// <summary>
        ///  How many times per week do you eat with your family?
        /// 1 - [0] 2 - [1-3] 3 - [4+]
        /// </summary>
        public byte? EatWithFamilyDays { get; set; }

        /// <summary>
        /// Do you exercise for about 20 minutes 3 or more days a week?
        /// 1 - Yes 2 - No
        /// </summary>
        public byte? C20MinExerFor3Days { get; set; }

        /// <summary>
        /// How intense is your exercise?
        /// 1- Heavy 2 - Moderate 3. Light
        /// </summary>
        public byte? ExerIntensity { get; set; }

        /// <summary>
        /// how would you rate your bodily pain?
        /// 1- Heavy 2 - Moderate 3. Light
        /// </summary>
        public byte? BodilyPain { get; set; }

        /// <summary>
        /// Can you go to places of walking distance without help?
        /// 1 - Yes 2 - No
        /// </summary>
        public byte? WalkWithoutHelp { get; set; }

        /// <summary>
        /// Can you prepare your own meals, housework, or shopping without help?
        /// 1- Yes 2 - No
        /// </summary>
        public byte? DoThingsWithoutHelp { get; set; }

        /// <summary>
        /// Do you need help eating, bathing, dressing, or getting around the house?
        /// 1- Yes 2 - No
        /// </summary>
        public byte? NeedHelpWithBasics { get; set; }

        /// <summary>
        /// Do you have memory problems?
        /// 1- Yes 2 - No
        /// </summary>
        public byte? MemoryProblems { get; set; }

        /// <summary>
        /// Is urination problem for you?
        /// 1- Yes 2 - No
        /// </summary>
        public byte? UrineProblems { get; set; }

        /// <summary>
        /// Have you fallen two or more times?
        /// 1- Yes 2 - No
        /// </summary>
        public byte? Fallen2Times { get; set; }

        /// <summary>
        /// Are you afraid of falling?
        /// 1- Yes 2 - No
        /// </summary>
        public byte? AfraidOfFalling { get; set; }

        /// <summary>
        /// Are there loose carpets, poor lighting, lack of handrails or grab bars in your home?
        /// 1- Yes 2 - No
        /// </summary>
        public byte? SafetyIssuesAtHome { get; set; }

        /// <summary>
        /// Do you always fasten your seatbelt when you are in a car?
        /// 1- Yes 2 - No
        /// </summary>
        public byte? FastenSeatbelt { get; set; }

        /// <summary>
        ///  Have you used tobacco (a cig, cigar, or chew) over 100 times?
        /// 1- Yes 2 - No
        /// </summary>
        public byte? UsedTob100Times { get; set; }

        /// <summary>
        /// Have you used tobacco at all in the last 4 weeks?
        /// 1- Yes 2 - No
        /// </summary>
        public byte? TobInLast4Weeks { get; set; }

        /// <summary>
        /// How many times a day do you drink caffeinated beverages
        /// 1 - [0] 2 - [1-2] 3 - [3+]
        /// </summary>
        public byte? CaffeinatedBev { get; set; }

        /// <summary>
        ///  have a problem with alcohol?
        /// 1- Yes 2 - No
        /// </summary>
        public byte? ProbWithAlcohol { get; set; }

        /// <summary>
        /// How many alcoholic beverages do you consume a day?
        /// 1 - [0] 2 - [1-3] 3 - [4+]
        /// </summary>
        public byte? NoOfDrinks { get; set; }

        /// <summary>
        /// Have you ever abused drugs or prescriptions?
        /// 1- Yes 2 - No
        /// </summary>
        public byte? AbusedDrugs { get; set; }

        /// <summary>
        /// Comments
        /// </summary>
        public string Comments { get; set; }

        public Dictionary<string, string> Validate(Dictionary<string, string> validationResult)
        {
            ValidationUtility.Validate(HealthRating, 1, 3, "HealthRating", validationResult);
            ValidationUtility.Validate(HealthStatus, 1, 3, "HealthStatus", validationResult);
            ValidationUtility.Validate(TroubleTakingMeds, 1, 2, "TroubleTakingMeds", validationResult);
            ValidationUtility.Validate(BotheredbyDizzy, 1, 2, "BotheredbyDizzy", validationResult);
            ValidationUtility.Validate(BotheredbySex, 1, 2, "BotheredbySex", validationResult);
            ValidationUtility.Validate(BotheredbyEating, 1, 2, "BotheredbyEating", validationResult);
            ValidationUtility.Validate(BotheredbyTeeth, 1, 2, "BotheredbyTeeth", validationResult);
            ValidationUtility.Validate(BotheredbyTelephone, 1, 2, "BotheredbyTelephone", validationResult);
            ValidationUtility.Validate(BotheredbyTiredness, 1, 2, "BotheredbyTiredness", validationResult);
            ValidationUtility.Validate(UseHearingAids, 1, 2, "UseHearingAids", validationResult);
            ValidationUtility.Validate(FeelingDown, 1, 2, "General.FeelingDown", validationResult);
            ValidationUtility.Validate(LittleInterest, 1, 2, "General.LittleInterest", validationResult);
            ValidationUtility.Validate(BreakfastDays, 1, 3, "BreakfastDays", validationResult);
            ValidationUtility.Validate(NoOfSweetDrinks, 1, 3, "NoOfSweetDrinks", validationResult);
            ValidationUtility.Validate(FruitandVegServ, 1, 3, "FruitandVegServ", validationResult);
            ValidationUtility.Validate(EatWithFamilyDays, 1, 3, "EatWithFamilyDays", validationResult);
            ValidationUtility.Validate(C20MinExerFor3Days, 1, 2, "C20MinExerFor3Days", validationResult);
            ValidationUtility.Validate(ExerIntensity, 1, 3, "ExerIntensity", validationResult);
            ValidationUtility.Validate(BodilyPain, 1, 3, "BodilyPain", validationResult);
            ValidationUtility.Validate(WalkWithoutHelp, 1, 2, "WalkWithoutHelp", validationResult);
            ValidationUtility.Validate(DoThingsWithoutHelp, 1, 2, "DoThingsWithoutHelp", validationResult);
            ValidationUtility.Validate(NeedHelpWithBasics, 1, 2, "NeedHelpWithBasics", validationResult);
            ValidationUtility.Validate(MemoryProblems, 1, 2, "MemoryProblems", validationResult);
            ValidationUtility.Validate(UrineProblems, 1, 2, "UrineProblems", validationResult);
            ValidationUtility.Validate(Fallen2Times, 1, 2, "Fallen2Times", validationResult);
            ValidationUtility.Validate(AfraidOfFalling, 1, 2, "AfraidOfFalling", validationResult);
            ValidationUtility.Validate(SafetyIssuesAtHome, 1, 2, "SafetyIssuesAtHome", validationResult);
            ValidationUtility.Validate(FastenSeatbelt, 1, 2, "FastenSeatbelt", validationResult);
            ValidationUtility.Validate(UsedTob100Times, 1, 2, "UsedTob100Times", validationResult);
            ValidationUtility.Validate(TobInLast4Weeks, 1, 2, "TobInLast4Weeks", validationResult);
            ValidationUtility.Validate(CaffeinatedBev, 1, 3, "CaffeinatedBev", validationResult);
            ValidationUtility.Validate(ProbWithAlcohol, 1, 2, "ProbWithAlcohol", validationResult);
            ValidationUtility.Validate(NoOfDrinks, 1, 3, "NoOfDrinks", validationResult);
            ValidationUtility.Validate(AbusedDrugs, 1, 2, "AbusedDrugs", validationResult);

            return validationResult;
        }

    }
}
