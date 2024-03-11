using Intervent.Web.DTO;

namespace InterventWebApp
{
    public class ReportsDashboardModel
    {
        public string Name { get; set; }

        public bool hasHRA { get; set; }

        public String hraCompleteDate { get; set; }

        public double? CAC { get; set; }

        public WellnessScoreInfo wellnessScoreInfo { get; set; }

        public double? PeerAverage { get; set; }

        public AgeInfo ageInfo { get; set; }

        public int healthMeasurementRisks { get; set; }

        public int lifetimeRisks { get; set; }

        public int otherModifiableRisks { get; set; }

        public int keyActionSteps { get; set; }

        public string picture { get; set; }

        public byte? pregnant { get; set; }

        public bool showNote { get; set; }

        public RisksModel risks { get; set; }

        public HRAGoalsDto goals { get; set; }

        public int? gender { get; set; }
        public string dob { get; set; }
        public int hraId { get; set; }
        public string assessmentName { get; set; }
        public int? hraVer { get; set; }
        public int? integrationWith { get; set; }
        public bool showSelfScheduling { get; set; }

    }

    public class ReportsModel
    {
        public WellnessScoreInfo wellnessScoreInfo { get; set; }
    }

    public class DrSummaryModel
    {
        public double? BMI { get; set; }

        public UserDto user { get; set; }

        public double? CAC { get; set; }

        public AgeInfo ageInfo { get; set; }

        public string wellnessheaderText { get; set; }

        public byte? pregnant { get; set; }

        public string arterialAgeText { get; set; }

        public string calciumRiskText { get; set; }

        public WellnessScoreInfo wellnessScoreInfo { get; set; }

        public double? PeerAverage { get; set; }

        public ListKeyActionStepsResponse keyActionSteps { get; set; }

        public string HraCompleteDate { get; set; }

        public string HraLabSource { get; set; }

        public string PrevHraCompleteDate { get; set; }

        public string BaseUrl { get; set; }

        public MedicalConditionsandRiskFactors medicalConditionsandRiskFactors { get; set; }

        public List<MeasurementsandGoals> measurementsandGoals { get; set; }

        public List<MeasurementsandGoals> initialMeasurementsandGoals { get; set; }

        public List<CheckupsandGoals> checkupsandGoals { get; set; }

        public List<ImmunizationandGoals> immunizationandGoals { get; set; }

        public DrReferralModel DrReferralModel { get; set; }

        public bool hadFollowUp { get; set; }

        public int? PreviousYearScheduledCoachingSession { get; set; }

        public int? PreviousYearCompletedCoachingSession { get; set; }

        public bool showNote { get; set; }

        public bool GINAQuestion { get; set; }

        public bool isDiaborPreDiab { get; set; }

        public List<AssignedKit> PreviousYearAssignedKitsmodel { get; set; }

        public bool clientNameInReport { get; set; }

        public string organizationName { get; set; }

        public string dateFormat { get; set; }

        public string orgContactEmail { get; set; }

        public string assessmentName { get; set; }

        public bool coachingProgram { get; set; }

        public bool selfHelpProgram { get; set; }

        public string dob { get; set; }

        public int hraId { get; set; }

        public int? integrationWith { get; set; }


    }

    public class MedicalConditionsandRiskFactors
    {
        public List<string> highRisk { get; set; }

        public List<string> missingRisk { get; set; }

        public List<string> noRisk { get; set; }
    }

    public class MeasurementsandGoals
    {
        public string Name { get; set; }

        public string Measurement { get; set; }

        public string Goal { get; set; }

        public string Unit { get; set; }
    }

    public class CheckupsandGoals
    {
        public string Name { get; set; }

        public int Status { get; set; }

        public string StatusMessage { get; set; }

        public string Recommended { get; set; }
    }

    public class AWVReportGoals
    {
        public string Name { get; set; }

        public string StatusMessage { get; set; }

        public string Recommended { get; set; }

        public string ClassName { get; set; }
    }

    public class AWVReportPlan : AWVReportGoals
    {
        public string NextDue { get; set; }

        public string LastReceived { get; set; }

    }

    public class HealthReferrals
    {
        public string Text { get; set; }

        public string Link { get; set; }

        public HealthReferrals(string text)
        {
            Text = text;
        }

        public HealthReferrals(string text, string link)
        {
            Text = text;
            Link = link;
        }
    }

    public class ImmunizationandGoals
    {
        public string Name { get; set; }

        public int Status { get; set; }

        public string StatusMessage { get; set; }

        public string Recommended { get; set; }
    }

    public class ScorecardModel
    {
        public WellnessScoreInfo wellnessScoreInfo { get; set; }

        public IList<ScoreCardItem> PHistoryItems { get; set; }

        public IList<ScoreCardItem> FHistoryItems { get; set; }

        public IList<ScoreCardItem> LFactorsItems { get; set; }

        public IList<ScoreCardItem> CMItems { get; set; }

        public IList<ScoreCardItem> PEItems { get; set; }

        public string DateFormat { get; set; }
    }

    public class ScoreCardItem
    {
        public string Name { get; set; }

        public string riskClass { get; set; }

        public string riskText { get; set; }

    }

    public class DrReferralModel
    {
        public List<string> referrals { get; set; }
    }

    public class OverweightInfoModel
    {
        public double? BMI { get; set; }

        public float? Waist { get; set; }
    }

    public class DiabetesInfoModel
    {
        public bool Diabetes { get; set; }
    }

    public class TobaccoInfoModel
    {
        public byte? SmokeCig { get; set; }

        public byte? OtherTobacco { get; set; }

        public byte? SmokelessTob { get; set; }

        public byte? Cigar { get; set; }

        public byte? Pipe { get; set; }
    }

    public class NutritionGoalModel
    {
        public string nutGoalIntro { get; set; }

        public string NutMealPlan { get; set; }

        public string FirstNutPlan { get; set; }

        public string[,] FirstNutPlanArray { get; set; }

        public string SecondNutPlan { get; set; }

        public string[,] SecondNutPlanArray { get; set; }

        public string meatTitle { get; set; }

        public string milkTitle { get; set; }

        public string fruitTitle { get; set; }

        public string nonstarchTitle { get; set; }

        public string startchTitle { get; set; }

        public string grainTitle { get; set; }

        public bool NutMaint { get; set; }

        public string NutMaintMealPlan { get; set; }

        public bool NutLowBMI { get; set; }

        public string NutLowBMIMealPlan { get; set; }

        public short? NutFatNum { get; set; }

        public int NewNutFatNum { get; set; }

        public bool NutLtFat { get; set; }

        public short? NutLtFatNum { get; set; }

        public int NewNutLtFatNum { get; set; }

        public bool NutUnderWeight { get; set; }

        public short? NutSatFatNum { get; set; }

        public bool NutLtSatFat { get; set; }

        public short? NutLtSatFatNum { get; set; }

        public string NutTransFatNum { get; set; }

        public string NutCholNum { get; set; }

        public bool NutDrinkWater { get; set; }

        public short? NutDrinkWaterNum { get; set; }

        public short? NutSugNum { get; set; }

        public float? NutStcal { get; set; }

        public float? NutLtcal { get; set; }

        public int NutCarbFrom { get; set; }

        public int NutCarbTo { get; set; }

        public int NutLtCarbFrom { get; set; }

        public int NutLtCarbTo { get; set; }

        public byte programType { get; set; }

        public byte Gender { get; set; }

        public IList<string> nutRecommendations;

        public bool onlymeal { get; set; }

        public bool onlydiet { get; set; }
    }

    public class PhysicalActivityGoalModel
    {
        public string ExerPlan { get; set; }

        public string[,] ExerPlanArray { get; set; }

        public byte programType { get; set; }

        public IList<string> exGuidelines;

        public string HRADate { get; set; }

        public bool hasActivePortal { get; set; }
    }

    public class StressGoalModel
    {
        public IList<string> stressGoalContent;
    }

    public class MedicationGoal
    {
        public IList<string> medicationGoalContent;
    }

    public class TobaccoGoalModel
    {
        public IList<string> tobaccoGoalContent;
    }

    public class SupplementGoalModel
    {
        public IList<string> supplementGoalContent;
    }

    public class ExamsGoalModel
    {
        public List<CheckupsandGoals> checkupsandGoals { get; set; }

        public List<ImmunizationandGoals> immunizationandGoals { get; set; }
    }

    public class RecommendationsModel
    {
        public bool hasProgram { get; set; }

        public int programType { get; set; }

        public NutritionGoalModel nutritionModel { get; set; }

        public PhysicalActivityGoalModel physicalActivityModel { get; set; }

        public DrReferralModel drReferralModel { get; set; }

        public byte? pregnant { get; set; }

        public string dob { get; set; }

        public bool carePlan { get; set; }

        public string nextApptDate { get; set; }

        public int hraId { get; set; }

        public bool coachingProgram { get; set; }

        public string userStatus { get; set; }



    }

    public class RiskChartsModel
    {
        public string reportTitle { get; set; }

        public List<RiskChart> riskCharts { get; set; }
    }

    public class RiskChart
    {
        public string riskName { get; set; }

        public double goalValue0 { get; set; }

        public string goalText0 { get; set; }

        public float currentValue { get; set; }

        public string currentYear { get; set; }

        public string currentText { get; set; }

        public float goalValue1 { get; set; }

        public string goalText1 { get; set; }

        public float? goalValue2 { get; set; }

        public string goalText2 { get; set; }

        public string label { get; set; }

        public string includeChar { get; set; }

        public string color1 { get; set; }

        public string color2 { get; set; }

        public float? startValue { get; set; }

        public string startText { get; set; }

        public string startYear { get; set; }

        public string color3 { get; set; }
    }

    public class HRASummaryModel
    {
        public int hraid { get; set; }

        public bool hasSummary { get; set; }

        public int? HRAVer { get; set; }

        public int? integrationWith { get; set; }

        public string AssessmentName { get; set; }

        public WellnessScoreInfo wellnessScoreInfo { get; set; }

        public double? CAC { get; set; }

        public double? PeerAverage { get; set; }

        public byte? Gender { get; set; }

        public AgeInfo ageInfo { get; set; }

        public RisksModel risks { get; set; }

        public int healthMeasurementRisks { get; set; }

        public int lifetimeRisks { get; set; }

        public int otherModifiableRisks { get; set; }

        public int keyActionSteps { get; set; }

        public double? PAISummary { get; set; }

        public double? MeasuredCRF { get; set; }

        public double? CRF { get; set; }

        public MedicalConditionsandRiskFactors medicalConditionsandRiskFactors { get; set; }

        public List<MeasurementsandGoals> measurementsandGoals { get; set; }

        public List<CheckupsandGoals> checkupsandGoals { get; set; }

        public List<ImmunizationandGoals> immunizationandGoals { get; set; }

        public NutritionGoalModel nutritionGoal { get; set; }

        public PhysicalActivityGoalModel physicalActivityGoal { get; set; }

        public DrReferralModel drReferralGoal { get; set; }

        public TobaccoGoalModel tobaccoGoal { get; set; }

        public StressGoalModel stressGoal { get; set; }

        public MedicationGoal medicationGoal { get; set; }

        public SupplementGoalModel supplementGoal { get; set; }
    }

    public class RisksModel
    {
        public HDSRisks hdsRisk { get; set; }

        public BPRisk bpRisk { get; set; }

        public CTRisk ctRisk { get; set; }

        public DBRisk diabetesRisk { get; set; }

        public OBRisk overweightRisk { get; set; }

        public MetRisk metRisk { get; set; }

        public CancerRisk cancerRisk { get; set; }

        public PARisk paRisk { get; set; }

        public ADARisk adaRisk { get; set; }

        public NutritionRisk nutRisk { get; set; }

        public SafetyRisks safetyRisk { get; set; }

        public StressRisk stressRisk { get; set; }

        public TobaccoRisk tobaccoRisk { get; set; }
    }

    public class HDSRisks
    {
        public string chdRiskText { get; set; }

        public RiskChart chdRiskChart { get; set; }

        public string tenYearRiskText { get; set; }

        public RiskChart tenYearRiskChart { get; set; }

        public string lifetimeRiskText { get; set; }

        public RiskChart lifetimeRiskChart { get; set; }

        public string calciumRiskText { get; set; }

        public string calciumScore { get; set; }

        public RiskChart calciumRiskChart { get; set; }

        public int? risk { get; set; }

        public string riskText { get; set; }
    }

    public class BPRisk
    {
        public RiskChart sbpRiskChart { get; set; }

        public RiskChart dbpRiskChart { get; set; }

        public int? risk { get; set; }

        public int? preRisk { get; set; }

        public string riskInfoText { get; set; }

        public string riskText { get; set; }
    }

    public class CTRisk
    {
        public RiskChart hdlRiskChart { get; set; }

        public RiskChart ldlRiskChart { get; set; }

        public RiskChart trigRiskChart { get; set; }

        public RiskChart tcRiskChart { get; set; }

        public int? risk { get; set; }

        public string riskInfoText { get; set; }

        public string riskText { get; set; }
    }

    public class MetRisk
    {
        public int? risk { get; set; }

        public string riskIntro { get; set; }

        public IList<string> riskText { get; set; }
    }

    public class DBRisk
    {
        public RiskChart glucChart { get; set; }

        public RiskChart a1cChart { get; set; }

        public int? risk { get; set; }

        public int? preRisk { get; set; }

        public string riskText { get; set; }
    }

    public class CancerRisk
    {
        public string riskIntro { get; set; }

        public int? risk { get; set; }

        public IList<string> riskText { get; set; }
    }

    public class OBRisk
    {
        public int? risk { get; set; }

        public string riskText { get; set; }

        public string riskInfoText { get; set; }

        public RiskChart riskChart { get; set; }

        public double safeWeight { get; set; }
    }

    public class PARisk
    {
        public int? risk { get; set; }

        public string riskText { get; set; }

        public string riskInfoText { get; set; }

        public RiskChart riskChart { get; set; }

        public string riskInfoTextPAI { get; set; }

        public RiskChart riskChartPAI { get; set; }

        public string color { get; set; }
    }

    public class ADARisk
    {
        public int? risk { get; set; }

        public string riskText { get; set; }

        public string riskInfoText { get; set; }

        public RiskChart riskChart { get; set; }

        public string riskInfoTextADA { get; set; }

        public RiskChart riskChartADA { get; set; }

        public string color { get; set; }

        public bool GINAQuestion { get; set; }

        public bool isDiaborPreDiab { get; set; }

    }

    public class SafetyRisks
    {
        public string riskIntro { get; set; }

        public int? risk { get; set; }

        public IList<string> riskText { get; set; }
    }

    public class StressRisk
    {
        public int? risk { get; set; }

        public string riskText { get; set; }

        public string riskInfoText { get; set; }
    }

    public class NutritionRisk
    {
        public string riskIntro { get; set; }

        public int? risk { get; set; }

        public IList<string> riskText { get; set; }

        public string riskInfoText { get; set; }

        public string goalText { get; set; }
    }

    public class TobaccoRisk
    {
        public int? risk { get; set; }

        public string riskText { get; set; }

        public string riskInfoText { get; set; }

        public RiskChart riskChart { get; set; }
    }

    public class WellnessScoreInfo
    {
        public decimal? wellnessScore { get; set; }

        public double? newWellnessScore { get; set; }

        public string wellnessHead { get; set; }

        public string wellnessText { get; set; }

        public string previousWellnessText { get; set; }

        public double? previousnewWellnessScore { get; set; }

        public DateTime? previousHraDate { get; set; }
    }


    public class FollowupReport
    {
        public bool hasSummary { get; set; }

        public string UserName { get; set; }

        public string Address { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public DateTime? DOB { get; set; }

        public string Picture { get; set; }

        public DateTime? ProgramStartDate { get; set; }

        public DateTime? AssessmentDate { get; set; }

        public string FollowupLabSource { get; set; }

        public int? ScheduledCoachingSession { get; set; }

        public int? CompletedCoachingSession { get; set; }

        public List<AssignedKit> AssignedKitsmodel { get; set; }

        public HDSRisks hdsRisks { get; set; }

        public CTRisk ctRisk { get; set; }

        public OBRisk overweightRisk { get; set; }

        public BPRisk bpRisk { get; set; }

        public DBRisk diabetesRisk { get; set; }

        public TobaccoRisk tobaccoRisk { get; set; }

        public List<String> referrals { get; set; }

        public List<SurveyQuestionDto> SurveyQuestions { get; set; }

        public bool IsSurveyCompleted { get; set; }

        public int? NoOfWeeks { get; set; }

        public int? PreviousYearScheduledCoachingSession { get; set; }

        public int? PreviousYearCompletedCoachingSession { get; set; }

        public List<AssignedKit> PreviousYearAssignedKitsmodel { get; set; }

        public bool reportGenerated { get; set; }

        public string reportGeneratedBy { get; set; }

        public string reportGeneratedOn { get; set; }

        public string labDataAlertNote { get; set; }

        public bool hasAdminRole { get; set; }

        public bool needCareplanApproval { get; set; }

        public int? gender { get; set; }

        public string programStartDateFormat { get; set; }

        public string dateFormat { get; set; }

        public string dob { get; set; }

        public string orgContactEmail { get; set; }

        public string DateFormat { get; set; }
    }

    public class FollowUpSummaryModel
    {
        public FollowupReport FollowUpReport { get; set; }
    }

    public class SurveyResponseModel
    {
        public List<SurveyResponseDto> SurveyResponse { get; set; }

        public bool EligibleForIncentive { get; set; }

        public string Comments { get; set; }
    }

    public class PrintableDetailedPlanModel
    {
        public UserDto User { get; set; }

        public HRADto HRA { get; set; }

        public DrReferralModel DrReferralGoal { get; set; }

        public NutritionGoalModel NutritionGoal { get; set; }

        public PhysicalActivityGoalModel PhysicalActivityGoal { get; set; }

        public TobaccoGoalModel TobaccoGoal { get; set; }

        public StressGoalModel StressGoal { get; set; }

        public MedicationGoal MedicationGoal { get; set; }

        public SupplementGoalModel SupplementGoal { get; set; }

        public SummaryCarePlanModel Stroke { get; set; }

        public SummaryCarePlanModel BloodPressure { get; set; }

        public SummaryCarePlanModel CVDRisk { get; set; }

        public SummaryCarePlanModel WeightManagement { get; set; }

        public int HealthMeasurementRisks { get; set; }

        public string ProviderComments { get; set; }

        public string CoachComments { get; set; }

        public int LifetimeRisks { get; set; }

        public int OtherModifiableRisks { get; set; }

        public RisksModel Risks { get; set; }

        public bool reportGenerated { get; set; }

        public string reportGeneratedBy { get; set; }

        public string reportGeneratedOn { get; set; }

        public bool needCareplanApproval { get; set; }

        public int? hraVer { get; set; }

        public int? integrationWith { get; set; }

        public string assessmentName { get; set; }
    }

    public class SummaryCarePlansModel
    {
        public SummaryCarePlanModel bloodPressure { get; set; }

        public SummaryCarePlanModel stroke { get; set; }

        public SummaryCarePlanModel cvdRisk { get; set; }

        public SummaryCarePlanModel weightManagement { get; set; }

    }

    public class SummaryCarePlanModel
    {
        public IList<string> numbersandGoals { get; set; }

        public IList<string> healthyLifestyle { get; set; }

        public IList<string> selfManagement { get; set; }

        public IList<string> regularMedicalCare { get; set; }

        public bool isPrintableReport { get; set; }

    }
}