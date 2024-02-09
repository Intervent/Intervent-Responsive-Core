namespace Intervent.Web.DTO.AWV
{
    /// <summary>
    /// Health Risk Screening
    /// </summary>
    public class HealthRiskScreensDto
    {
        /// <summary>
        /// Depression Screening
        /// </summary>
        public DepressionScreensDto DepressionScreening { get; set; }

        /// <summary>
        /// Home Screening (Includes Home Safety, Functional Activities, Fall Risk Screens and Hearing Loss Screen)
        /// </summary>
        public HomeScreensDto HealthScreening { get; set; }

        /// <summary>
        /// Osteoporosis Risk Screen
        /// </summary>
        public OsteoporosisScreensDto OsteoporosisScreens { get; set; }

        /// <summary>
        /// Urinary Incontinence Screen
        /// </summary>
        public UrinaryScreensDto UrinaryScreening { get; set; }

        public Dictionary<string, string> Validate(Dictionary<string, string> validateResult)
        {
            if (DepressionScreening != null)
                validateResult = DepressionScreening.Validate(validateResult);
            if (HealthScreening != null)
                validateResult = HealthScreening.Validate(validateResult);
            if (OsteoporosisScreens != null)
                validateResult = OsteoporosisScreens.Validate(validateResult);
            if (UrinaryScreening != null)
                validateResult = UrinaryScreening.Validate(validateResult);
            return validateResult;
        }
    }

    #region Depression Screening
    /// <summary>
    /// Depression Screening
    /// </summary>
    public class DepressionScreensDto
    {
        /// <summary>
        /// Little interest or pleasure in doing things       
        /// 1. Not at all 2. Several days 3. More than half the days 4. Nearly every day
        /// </summary>
        public byte? LittleInterest { get; set; }

        /// <summary>
        /// Feeling down, depressed, or hopeless
        /// 1. Not at all 2. Several days 3. More than half the days 4. Nearly every day
        /// </summary>
        public byte? FeelingDown { get; set; }

        /// <summary>
        /// Trouble falling or staying asleep, or sleeping too much
        /// 1. Not at all 2. Several days 3. More than half the days 4. Nearly every day
        /// </summary>
        public byte? SleepTrouble { get; set; }

        /// <summary>
        /// Feeling tired or having little energy
        /// 1. Not at all 2. Several days 3. More than half the days 4. Nearly every day
        /// </summary>
        public byte? Tired { get; set; }

        /// <summary>
        /// Poor appetite or overeating
        /// 1. Not at all 2. Several days 3. More than half the days 4. Nearly every day
        /// </summary>
        public byte? EatingIssue { get; set; }

        /// <summary>
        /// Feeling bad about yourself — or that you are a failure or have let yourself or your family down
        /// 1. Not at all 2. Several days 3. More than half the days 4. Nearly every day
        /// </summary>
        public byte? FeelingBad { get; set; }

        /// <summary>
        /// Trouble concentrating on things, such as reading the newspaper or watching television
        /// 1. Not at all 2. Several days 3. More than half the days 4. Nearly every day
        /// </summary>
        public byte? TroubleConc { get; set; }

        /// <summary>
        /// Moving or speaking so slowly that other people could have noticed? Or the opposite — being so fidgety or restless that you have been moving around a lot more than usual
        /// 1. Not at all 2. Several days 3. More than half the days 4. Nearly every day
        /// </summary>
        public byte? TroubleMoving { get; set; }

        /// <summary>
        /// Thoughts that you would be better off dead or of hurting yourself in some way
        /// 1. Not at all 2. Several days 3. More than half the days 4. Nearly every day
        /// </summary>
        public byte? BetterDead { get; set; }

        /// <summary>
        /// how difficult have these problems made it for you to do your work, take care of things at home, or get along with other people
        /// 1. Not difficult at all 2. Somewhat difficult 3. Very difficult 4. Extremely difficult
        /// </summary>
        public byte? DiffLevel { get; set; }

        /// <summary>
        /// PHQ9 Score 
        /// 1. None 2. Mild 3. Moderate 4. Mod - Severe (15-19) 5. Severe (20-27) 
        /// </summary>
        public byte? PHQ9_Score { get; set; }

        /// <summary>
        /// PHQ9 Score (Provider Assessment)
        /// 1. Advised /Reviewed 2. Refer to PCP
        /// </summary>
        public byte? PHQ9_ProviderScore { get; set; }

        public Dictionary<string, string> Validate(Dictionary<string, string> validateResult)
        {
            ValidationUtility.Validate(LittleInterest, 1, 4, "DepressionScreens.LittleInterest", validateResult);
            ValidationUtility.Validate(FeelingBad, 1, 4, "FeelingBad", validateResult);
            ValidationUtility.Validate(FeelingDown, 1, 4, "DepressionScreens.FeelingDown", validateResult);
            ValidationUtility.Validate(SleepTrouble, 1, 4, "SleepTrouble", validateResult);
            ValidationUtility.Validate(Tired, 1, 4, "Tired", validateResult);
            ValidationUtility.Validate(EatingIssue, 1, 4, "EatingIssue", validateResult);
            ValidationUtility.Validate(TroubleConc, 1, 4, "TroubleConc", validateResult);
            ValidationUtility.Validate(TroubleMoving, 1, 4, "TroubleMoving", validateResult);
            ValidationUtility.Validate(BetterDead, 1, 4, "BetterDead", validateResult);
            ValidationUtility.Validate(DiffLevel, 1, 4, "DiffLevel", validateResult);
            ValidationUtility.Validate(PHQ9_Score, 1, 5, "PHQ9_Score", validateResult);
            ValidationUtility.Validate(PHQ9_ProviderScore, 1, 2, "PHQ9_ProviderScore", validateResult);

            return validateResult;
        }
    }
    #endregion

    #region OsteoporosisScreens
    /// <summary>
    /// Osteoporosis Risk Screen
    /// </summary>
    public class OsteoporosisScreensDto
    {
        /// <summary>
        /// Do you have documented Osteoporosis(733.00) or Osteopenia(733.90)
        /// 1. Yes 2. No
        /// </summary>
        public byte? Osteoporosis { get; set; }

        /// <summary>
        /// Are you older than 65 years of Age
        /// 1. Yes 2. No
        /// </summary>
        public byte? Olderthan65 { get; set; }

        /// <summary>
        /// Have you ever Had a Bone densitometry
        /// 1. Yes 2. No
        /// </summary>
        public byte? BoneDensitometry { get; set; }

        /// <summary>
        /// Do you take or have you ever been on cortisone, prednisone, or other steroids for greater than 3 months during you life (V58.65 and 733.09)
        /// 1. Yes 2. No
        /// </summary>
        public byte? OnCartisone { get; set; }

        /// <summary>
        /// Are you thin, small boned or weigh less than 127lbs
        /// 1. Yes 2. No
        /// </summary>
        public byte? SmallBoned { get; set; }

        /// <summary>
        /// Do you drink alcohol beverages and Smoke
        /// 1. Yes 2. No
        /// </summary>
        public byte? DrinkorSmoke { get; set; }

        /// <summary>
        /// Does anyone in your immediate family have Osteoporosis(V17.81)
        /// 1. Yes 2. No
        /// </summary>
        public byte? FHOsteoporosis { get; set; }

        /// <summary>
        /// Do you live an inactive lifestyle (lack of exercise)
        /// 1. Yes 2. No
        /// </summary>
        public byte? InactiveLife { get; set; }

        /// <summary>
        /// Medication: Blood Thinning i.e Coumadin
        /// 1. Yes
        /// </summary>
        public byte? BloodThinMed { get; set; }

        /// <summary>
        /// Medication: Cancer i.e Chemo, Radiation
        /// 1. Yes
        /// </summary>
        public byte? CancerMed { get; set; }

        /// <summary>
        /// Medication: Rheumatism i.e Methotrexate
        /// 1. Yes
        /// </summary>
        public byte? RheumatismMed { get; set; }

        /// <summary>
        /// Medication: Seizure i.e Dilantin
        /// 1. Yes
        /// </summary>
        public byte? SeizureMed { get; set; }

        /// <summary>
        /// Medication: Water Pill i.e Furosemide
        /// 1. Yes
        /// </summary>
        public byte? WaterPill { get; set; }

        /// <summary>
        /// Medication: Thyroid i.e. Synthroid, Levothyroid
        /// 1. Yes
        /// </summary>
        public byte? ThyroidMed { get; set; }

        /// <summary>
        /// Medication: Osteoporosis
        /// 1. Yes
        /// </summary>
        public byte? OstMed { get; set; }

        /// <summary>
        /// Disorder: Alcoholism(303.90)
        /// 1. Yes
        /// </summary>
        public byte? Alcohololism { get; set; }

        /// <summary>
        /// Disorder: Kidney Stones(592.0)
        /// 1. Yes
        /// </summary>
        public byte? KidStone { get; set; }

        /// <summary>
        /// Disorder: Cancer
        /// 1. Yes
        /// </summary>
        public byte? Cancer { get; set; }

        /// <summary>
        /// Disorder: Malabsorption(263.9)
        /// 1. Yes
        /// </summary>
        public byte? Malabsorption { get; set; }

        /// <summary>
        /// Disorder: Eating Disorder(277.0)
        /// 1. Yes
        /// </summary>
        public byte? EastDis { get; set; }

        /// <summary>
        /// Disorder: Rheumatism(714.0)
        /// 1. Yes
        /// </summary>
        public byte? Rheumatism { get; set; }

        /// <summary>
        /// Disorder: Hyperparathyroidism(252.0)
        /// 1. Yes
        /// </summary>
        public byte? HypThy { get; set; }

        /// <summary>
        /// Disorder: Thyroid(244.9)
        /// 1. Yes
        /// </summary>
        public byte? ThyDis { get; set; }

        /// <summary>
        /// Do you have a backache(724.5),humped back (kyphosis)(737.41), back curvature (scoliosis)(737.43)
        /// 1. Yes 2. No
        /// </summary>
        public byte? Backache { get; set; }

        /// <summary>
        /// Do you have an abnormal posture(781.92)
        /// 1. Yes 2. No
        /// </summary>
        public byte? Posture { get; set; }

        /// <summary>
        /// Have you lost over 1 inch in height since age 25(781.91)
        /// 1. Yes 2. No
        /// </summary>
        public byte? LostHeight { get; set; }

        /// <summary>
        /// Have you had a compression fracture of the back(733.13)
        /// 1. Yes 2. No
        /// </summary>
        public byte? CompFrac { get; set; }

        /// <summary>
        /// Have you had Hip Replacement Surgery
        /// 1. Yes 2. No
        /// </summary>
        public byte? HipRep { get; set; }

        /// <summary>
        /// Have you broken any bones without much effort or trauma
        /// 1. Yes 2. No
        /// </summary>
        public byte? BrokenBones { get; set; }

        /// <summary>
        /// Have you stopped having periods (post menopausal)(V49.81)
        /// 1. Yes 2. No
        /// </summary>
        public byte? PostMeno { get; set; }

        /// <summary>
        /// Did your periods stop before age 45(256.3)
        /// 1. Yes 2. No
        /// </summary>
        public byte? PerBef45 { get; set; }

        /// <summary>
        /// Did you have your ovaries removed(256.2)
        /// 1. Yes 2. No
        /// </summary>
        public byte? OvRem { get; set; }

        /// <summary>
        /// Are you experiencing hot flashes, sleeplessness, headaches, lack of concentration,vaginal dryness, or decreased Libido(627.2)
        /// 1. Yes 2. No
        /// </summary>
        public byte? HotFlash { get; set; }

        /// <summary>
        /// Are you on Estrogen(V07.4)
        /// 1. Yes 2. No
        /// </summary>
        public byte? OnEstogen { get; set; }

        /// <summary>
        /// Osteoporosis Risk Score 
        /// 1. No further evaluation required 2. High Risk (>2) for Ost 3. DEXA recommended 4. DEXA ordered
        /// </summary>
        public byte? OstRiskScore { get; set; }

        public Dictionary<string, string> Validate(Dictionary<string, string> validateResult)
        {
            ValidationUtility.Validate(Osteoporosis, 1, 2, "OsteoporosisScrens.Osteoporosis", validateResult);
            ValidationUtility.Validate(Olderthan65, 1, 2, "Olderthan65", validateResult);
            ValidationUtility.Validate(BoneDensitometry, 1, 2, "BoneDensitometry", validateResult);
            ValidationUtility.Validate(OnCartisone, 1, 2, "OnCartisone", validateResult);
            ValidationUtility.Validate(SmallBoned, 1, 2, "SmallBoned", validateResult);
            ValidationUtility.Validate(DrinkorSmoke, 1, 2, "DrinkorSmoke", validateResult);
            ValidationUtility.Validate(FHOsteoporosis, 1, 2, "OsteoporosisScrens.FHOsteoporosis", validateResult);
            ValidationUtility.Validate(InactiveLife, 1, 2, "InactiveLife", validateResult);
            ValidationUtility.Validate(BloodThinMed, 1, 1, "BloodThinMed", validateResult);
            ValidationUtility.Validate(CancerMed, 1, 1, "CancerMed", validateResult);
            ValidationUtility.Validate(RheumatismMed, 1, 1, "RheumatismMed", validateResult);
            ValidationUtility.Validate(SeizureMed, 1, 1, "SeizureMed", validateResult);
            ValidationUtility.Validate(WaterPill, 1, 1, "WaterPill", validateResult);
            ValidationUtility.Validate(ThyroidMed, 1, 1, "ThyroidMed", validateResult);
            ValidationUtility.Validate(OstMed, 1, 1, "OstMed", validateResult);
            ValidationUtility.Validate(Alcohololism, 1, 1, "Alcohololism", validateResult);
            ValidationUtility.Validate(KidStone, 1, 1, "KidStone", validateResult);
            ValidationUtility.Validate(Cancer, 1, 1, "Cancer", validateResult);
            ValidationUtility.Validate(Malabsorption, 1, 1, "Malabsorption", validateResult);
            ValidationUtility.Validate(EastDis, 1, 1, "EastDis", validateResult);
            ValidationUtility.Validate(Rheumatism, 1, 1, "Rheumatism", validateResult);
            ValidationUtility.Validate(HypThy, 1, 1, "HypThy", validateResult);
            ValidationUtility.Validate(ThyDis, 1, 1, "ThyDis", validateResult);
            ValidationUtility.Validate(Backache, 1, 2, "Backache", validateResult);
            ValidationUtility.Validate(Posture, 1, 2, "Posture", validateResult);
            ValidationUtility.Validate(LostHeight, 1, 2, "LostHeight", validateResult);
            ValidationUtility.Validate(CompFrac, 1, 2, "CompFrac", validateResult);
            ValidationUtility.Validate(HipRep, 1, 2, "HipRep", validateResult);
            ValidationUtility.Validate(BrokenBones, 1, 2, "BrokenBones", validateResult);
            ValidationUtility.Validate(PostMeno, 1, 2, "PostMeno", validateResult);
            ValidationUtility.Validate(PerBef45, 1, 2, "PerBef45", validateResult);
            ValidationUtility.Validate(OvRem, 1, 2, "OvRem", validateResult);
            ValidationUtility.Validate(HotFlash, 1, 2, "HotFlash", validateResult);
            ValidationUtility.Validate(OnEstogen, 1, 2, "OnEstogen", validateResult);
            ValidationUtility.Validate(OstRiskScore, 1, 4, "OstRiskScore", validateResult);

            return validateResult;
        }

    }
    #endregion

    #region Home Screening
    /// <summary>
    /// Home Screening (Includes Home Safety, Functional Activities, Fall Risk Screens and Hearing Loss Screen)
    /// </summary>
    public class HomeScreensDto
    {
        /// <summary>
        /// Do you have throwrugs on hardwood floors in your house?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Home_Throwrugs { get; set; }

        /// <summary>
        /// Do you have pets that stay indoors?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Home_IndoorPets { get; set; }

        /// <summary>
        /// Does your house have smoke alarms and carbon monoxide detectors in good working order?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Home_SmokeAlarm { get; set; }

        /// <summary>
        /// Does your bathtub contain a safety measure such as a rubber mat or strips?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Home_BathtubSafety { get; set; }

        /// <summary>
        /// Is the area in front of your bathtub carpeted or protected by a bathmat with rubber backing?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Home_Bathmat { get; set; }

        /// <summary>
        /// Do you have night lights in your house?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Home_NightLight { get; set; }

        /// <summary>
        /// Do you have loose or frayed cords or overloaded electrical sockets in your house?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Home_LooseCords { get; set; }

        /// <summary>
        /// Do you unplug household appliances when not in use?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Home_UnplugAppl { get; set; }

        /// <summary>
        ///  Do you keep medicines in a safe place and have their directions clearly labeled?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Home_MedSafe { get; set; }

        /// <summary>
        /// Do you keep knives and other sharp objects put away in a safe place?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Home_KnifeSafe { get; set; }

        /// <summary>
        /// Do you keep poisons, chemical or other toxic substances put away in a safe place?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Home_ChemsSafe { get; set; }

        /// <summary>
        /// Do you have furniture (i.e coffee table) with sharp corners, or a rickety chair, that could cause injury?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Home_SharpFurn { get; set; }

        /// <summary>
        /// Home Safety Score
        /// 1. Safe (0-2) 2. Possibly Unsafe (3+)
        /// </summary>
        public byte? HomeSafetyScore { get; set; }

        /// <summary>
        /// Home Safety Score (Provider Assessment)
        /// 1. Advised /Reviewed 2. Refer to PCP
        /// </summary>
        public byte? HomeSafetyProviderScore { get; set; }

        /// <summary>
        /// Can you get out of bed by yourself?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Fun_GetOutofBed { get; set; }

        /// <summary>
        /// Can you dress yourself without help?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Fun_DressYourself { get; set; }

        /// <summary>
        /// Can you prepare your own meals?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Fun_PrepOwnMeals { get; set; }

        /// <summary>
        /// Do you do your own shopping?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Fun_OwnSHop { get; set; }

        /// <summary>
        /// Do you write checks and pay your own bills?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Fun_WriteCheck { get; set; }

        /// <summary>
        /// Do you drive or have other means of transportation for traveling outside your neighborhood?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Fun_Drive { get; set; }

        /// <summary>
        /// Are you able to keep track of appointments and family occasions?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Fun_KeepApp { get; set; }

        /// <summary>
        /// Are you able to take medicine according to directions, dosing, etc.?    
        /// 1. Yes 2. No
        /// </summary>
        public byte? Fun_TakeMed { get; set; }

        /// <summary>
        /// Are you able to keep track of current events?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Fun_KeepEvents { get; set; }

        /// <summary>
        /// Are you still able to play games of skill that you enjoy or work on a favorite hobby?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Fun_PlayGame { get; set; }

        /// <summary>
        /// Functional Activity Score
        /// 1. ADLs Normal (0-1) 2. Recodnized ADLs difficulties (2+)
        /// </summary>
        public byte? FunActivityScore { get; set; }

        /// <summary>
        /// Functional Activity Score (Provider Assessment)
        /// 1. Advised /Reviewed 2. Refer to PCP
        /// </summary>
        public byte? FunActivityProviderScore { get; set; }

        /// <summary>
        /// Do you notice Numbess in your feet?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Fall_Numbess { get; set; }

        /// <summary>
        /// Steps feel “heavy”
        /// 1. Yes 2. No
        /// </summary>
        public byte? Fall_HeavySteps { get; set; }

        /// <summary>
        /// Light-headed upon rising 
        /// 1. Yes 2. No
        /// </summary>
        public byte? Fall_LightHeaded { get; set; }

        /// <summary>
        /// Walking, difficulty start/stop 
        /// 1. Yes 2. No
        /// </summary>
        public byte? Fall_DiffStartStop { get; set; }

        /// <summary>
        /// Trouble getting our of chair
        /// 1. Yes 2. No
        /// </summary>
        public byte? Fall_TroubleGetOut { get; set; }

        /// <summary>
        /// Difficulty when walking
        /// 1. Yes 2. No
        /// </summary>
        public byte? Fall_DiffWalk { get; set; }

        /// <summary>
        /// Ever lose your balance
        /// 1. Yes 2. No
        /// </summary>
        public byte? Fall_LooseBalance { get; set; }

        /// <summary>
        /// Ever fallen in the past
        /// 1. Yes 2. No
        /// </summary>
        public byte? Fall_EverFall { get; set; }

        /// <summary>
        /// Fall Risk Score
        /// 1. Safe(0-1) 2. Possible Risk To fall (2+) 3. Get up and Go test preferred
        /// </summary>
        public byte? FallRiskScore { get; set; }

        /// <summary>
        /// Fall Risk Score (Provider Assessment)
        /// 1. Advised /Reviewed 2. Refer to PCP
        /// </summary>
        public byte? FallRiskProviderScore { get; set; }

        /// <summary>
        /// Do you have a problem hearing over the telephone?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Hear_ProbOverTel { get; set; }

        /// <summary>
        /// Do you have trouble following the conversation when two or more people talk at the same time?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Hear_TroubleFollConv { get; set; }

        /// <summary>
        /// Do people complain that you turn the TV or radio volume up too high?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Hear_AudioHighVol { get; set; }

        /// <summary>
        /// Do you have to strain to understand conversation?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Hear_StraintoUnderstand { get; set; }

        /// <summary>
        /// Do you have trouble hearing in a noisy background (i.e. party, movie theater)?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Hear_NoisyBack { get; set; }

        /// <summary>
        /// Do you find yourself asking people to repeat themselves?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Hear_AsktoRepeat { get; set; }

        /// <summary>
        /// Do many people you talk to seem to mumble, or not speak clearly?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Hear_PeopleMumble { get; set; }

        /// <summary>
        /// Do you misunderstand what others are saying and respond inappropriately?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Hear_Misunderstand { get; set; }

        /// <summary>
        /// Do you have trouble understanding the speech of women and children?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Hear_TroubleUnderstand { get; set; }

        /// <summary>
        ///  Do people get annoyed because you misunderstand what they say?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Hear_PeopleGetAnnoyed { get; set; }

        /// <summary>
        /// Hearing Loss Score
        /// 1. No hearning defects (0) 2. Possible hearning loss (confirm with audiometry) 1+
        /// </summary>
        public byte? HearingLossScore { get; set; }

        /// <summary>
        /// Hearing Loss Score (Provider Assessment)
        /// 1. Advised /Reviewed 2. Refer to PCP
        /// </summary>
        public byte? HearingLossProviderScore { get; set; }

        public Dictionary<string, string> Validate(Dictionary<string, string> validateResult)
        {
            ValidationUtility.Validate(Home_Throwrugs, 1, 2, "Home_Throwrugs", validateResult);
            ValidationUtility.Validate(Home_IndoorPets, 1, 2, "Home_IndoorPets", validateResult);
            ValidationUtility.Validate(Home_SmokeAlarm, 1, 2, "Home_SmokeAlarm", validateResult);
            ValidationUtility.Validate(Home_BathtubSafety, 1, 2, "Home_BathtubSafety", validateResult);
            ValidationUtility.Validate(Home_Bathmat, 1, 2, "Home_Bathmat", validateResult);
            ValidationUtility.Validate(Home_NightLight, 1, 2, "Home_NightLight", validateResult);
            ValidationUtility.Validate(Home_LooseCords, 1, 2, "Home_LooseCords", validateResult);
            ValidationUtility.Validate(Home_UnplugAppl, 1, 2, "Home_UnplugAppl", validateResult);
            ValidationUtility.Validate(Home_MedSafe, 1, 2, "Home_MedSafe", validateResult);
            ValidationUtility.Validate(Home_KnifeSafe, 1, 2, "Home_KnifeSafe", validateResult);
            ValidationUtility.Validate(Home_ChemsSafe, 1, 2, "Home_ChemsSafe", validateResult);
            ValidationUtility.Validate(Home_SharpFurn, 1, 2, "Home_SharpFurn", validateResult);
            ValidationUtility.Validate(HomeSafetyScore, 1, 2, "HomeSafetyScore", validateResult);
            ValidationUtility.Validate(HomeSafetyProviderScore, 1, 2, "HomeSafetyProviderScore", validateResult);
            ValidationUtility.Validate(Fun_GetOutofBed, 1, 2, "Fun_GetOutofBed", validateResult);
            ValidationUtility.Validate(Fun_DressYourself, 1, 2, "Fun_DressYourself", validateResult);
            ValidationUtility.Validate(Fun_PrepOwnMeals, 1, 2, "Fun_PrepOwnMeals", validateResult);
            ValidationUtility.Validate(Fun_OwnSHop, 1, 2, "Fun_OwnSHop", validateResult);
            ValidationUtility.Validate(Fun_WriteCheck, 1, 2, "Fun_WriteCheck", validateResult);
            ValidationUtility.Validate(Fun_Drive, 1, 2, "Fun_Drive", validateResult);
            ValidationUtility.Validate(Fun_KeepApp, 1, 2, "Fun_KeepApp", validateResult);
            ValidationUtility.Validate(Fun_TakeMed, 1, 2, "Fun_TakeMed", validateResult);
            ValidationUtility.Validate(Fun_KeepEvents, 1, 2, "Fun_KeepEvents", validateResult);
            ValidationUtility.Validate(Fun_PlayGame, 1, 2, "Fun_PlayGame", validateResult);
            ValidationUtility.Validate(FunActivityProviderScore, 1, 2, "FunActivityProviderScore", validateResult);
            ValidationUtility.Validate(FunActivityScore, 1, 2, "FunActivityScore", validateResult);
            ValidationUtility.Validate(Fall_Numbess, 1, 2, "Fall_Numbess", validateResult);
            ValidationUtility.Validate(Fall_HeavySteps, 1, 2, "Fall_HeavySteps", validateResult);
            ValidationUtility.Validate(Fall_LightHeaded, 1, 2, "Fall_LightHeaded", validateResult);
            ValidationUtility.Validate(Fall_DiffStartStop, 1, 2, "Fall_DiffStartStop", validateResult);
            ValidationUtility.Validate(Fall_TroubleGetOut, 1, 2, "Fall_TroubleGetOut", validateResult);
            ValidationUtility.Validate(Fall_DiffWalk, 1, 2, "Fall_DiffWalk", validateResult);
            ValidationUtility.Validate(Fall_LooseBalance, 1, 2, "Fall_LooseBalance", validateResult);
            ValidationUtility.Validate(FallRiskScore, 1, 3, "FallRiskScore", validateResult);
            ValidationUtility.Validate(FallRiskProviderScore, 1, 2, "FallRiskProviderScore", validateResult);
            ValidationUtility.Validate(Hear_ProbOverTel, 1, 2, "Hear_ProbOverTel", validateResult);
            ValidationUtility.Validate(Hear_TroubleFollConv, 1, 2, "Hear_TroubleFollConv", validateResult);
            ValidationUtility.Validate(Hear_AudioHighVol, 1, 2, "Hear_AudioHighVol", validateResult);
            ValidationUtility.Validate(Hear_AsktoRepeat, 1, 2, "Hear_AsktoRepeat", validateResult);
            ValidationUtility.Validate(Hear_StraintoUnderstand, 1, 2, "Hear_StraintoUnderstand", validateResult);
            ValidationUtility.Validate(Hear_NoisyBack, 1, 2, "Hear_NoisyBack", validateResult);
            ValidationUtility.Validate(Hear_PeopleMumble, 1, 2, "Hear_PeopleMumble", validateResult);
            ValidationUtility.Validate(Hear_Misunderstand, 1, 2, "Hear_Misunderstand", validateResult);
            ValidationUtility.Validate(Hear_TroubleUnderstand, 1, 2, "Hear_TroubleUnderstand", validateResult);
            ValidationUtility.Validate(Hear_PeopleGetAnnoyed, 1, 2, "Hear_PeopleGetAnnoyed", validateResult);
            ValidationUtility.Validate(HearingLossScore, 1, 2, "HearingLossScore", validateResult);
            ValidationUtility.Validate(HearingLossProviderScore, 1, 2, "HearingLossProviderScore", validateResult);

            return validateResult;
        }
    }
    #endregion

    #region Urinary Screening
    /// <summary>
    /// Urinary Incontinence Screen
    /// </summary>
    public class UrinaryScreensDto
    {
        /// <summary>
        /// Leak Urine: when you cough or sneeze?
        /// 1. Yes 2. No
        /// </summary>
        public byte? WhenCough { get; set; }

        /// <summary>
        /// Leak Urine: when you bend down or lift?
        /// 1. Yes 2. No
        /// </summary>
        public byte? WhenBend { get; set; }

        /// <summary>
        /// Leak Urine: when you walk quickly, jog or exercise?
        /// 1. Yes 2. No
        /// </summary>
        public byte? WhenExer { get; set; }

        /// <summary>
        /// Leak Urine: while you are undressing to use the toilet?
        /// 1. Yes 2. No
        /// </summary>
        public byte? WhenUndress { get; set; }

        /// <summary>
        /// Leak Urine: Do you get such a strong and urge to urinate that you leak urine before you reach the toilet?
        /// 1. Yes 2. No
        /// </summary>
        public byte? BefReachToilet { get; set; }

        /// <summary>
        /// Leak Urine: Do you have to rush to the bathroom because you get a sudden,strong need to urinate?
        /// 1. Yes 2. No
        /// </summary>
        public byte? RushtoBath { get; set; }

        /// <summary>
        /// Leakage Affect: Do household chores?
        /// 1. Yes 2. No
        /// </summary>
        public byte? AffHousChores { get; set; }

        /// <summary>
        /// Leakage Affect: participate inphysical recreation
        /// 1. Yes 2. No
        /// </summary>
        public byte? AffPhyRec { get; set; }

        /// <summary>
        /// Leakage Affect: participate in entertainment activities
        /// 1. Yes 2. No
        /// </summary>
        public byte? AffEnt { get; set; }

        /// <summary>
        /// Leakage Affect: travel by car more than 30 minutes from home
        /// 1. Yes 2. No
        /// </summary>
        public byte? Travel { get; set; }

        /// <summary>
        /// Leakage Affect: participate in social activties outside your house?
        /// 1. Yes 2. No
        /// </summary>
        public byte? Social { get; set; }

        /// <summary>
        /// Leakage Affect: Does leakage have you feeling frustrated,nervous or depressed
        /// 1. Yes 2. No
        /// </summary>
        public byte? FeelFrust { get; set; }

        /// <summary>
        /// Urinary Risk Score
        /// 1. No evaluation required 2. Refer to PCP for Urinary Stress 3. Refer to PCP for Urinary Bladded/Urge   
        /// </summary>
        public byte? UrnRiskScore { get; set; }

        public Dictionary<string, string> Validate(Dictionary<string, string> validateResult)
        {
            ValidationUtility.Validate(WhenCough, 1, 2, "WhenCough", validateResult);
            ValidationUtility.Validate(WhenBend, 1, 2, "WhenBend", validateResult);
            ValidationUtility.Validate(WhenUndress, 1, 2, "WhenUndress", validateResult);
            ValidationUtility.Validate(WhenExer, 1, 2, "WhenExer", validateResult);
            ValidationUtility.Validate(BefReachToilet, 1, 2, "BefReachToilet", validateResult);
            ValidationUtility.Validate(RushtoBath, 1, 2, "RushtoBath", validateResult);
            ValidationUtility.Validate(AffHousChores, 1, 2, "AffHousChores", validateResult);
            ValidationUtility.Validate(AffPhyRec, 1, 2, "AffPhyRec", validateResult);
            ValidationUtility.Validate(AffEnt, 1, 2, "AffEnt", validateResult);
            ValidationUtility.Validate(Travel, 1, 2, "Travel", validateResult);
            ValidationUtility.Validate(Social, 1, 2, "Social", validateResult);
            ValidationUtility.Validate(FeelFrust, 1, 2, "FeelFrust", validateResult);
            ValidationUtility.Validate(UrnRiskScore, 1, 2, "UrnRiskScore", validateResult);
            return validateResult;
        }
    }
    #endregion
}
