using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class HRAModel
    {
        public int HRAId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? CompleteDate { get; set; }

        public string HAPageSeqDone { get; set; }

        public bool GoalsGenerated { get; set; }

        public string RiskCode { get; set; }

        public float? UOMRisk { get; set; }

        public float? IVRisk { get; set; }

        public float? WellnessScore { get; set; }

        public int hraPercent { get; set; }

        public bool readOnly { get; set; }

        public bool isSouthUniversity { get; set; }

        public int UserId { get; set; }

        public string HRAPageSeq { get; set; }

        public string OrgContactEmail { get; set; }

        public string HRAValidity { get; set; }

        public int CreatedBy { get; set; }

        public int? AdminId { get; set; }

        public bool isPastHRA { get; set; }

        public bool HasActivePortal { get; set; }
    }

    public class MedicalConditionModel
    {
        public int participantId { get; set; }

        public int userId { get; set; }

        public int systemAdminId { get; set; }

        public int? userinProgramId { get; set; }

        public int? participantPortalId { get; set; }

        public int? hraId { get; set; }

        public int? integrationWith { get; set; }

        public string DateFormat { get; set; }

        public string ParticipantEmail { get; set; }

        public bool MailScoreCard { get; set; }

        public bool ShowPostmenopausal { get; set; }

        public int? AdminId { get; set; }

        public DateTime? hraCompleteDate { get; set; }

        public string hraPageSeqDone { get; set; }

        public string hraPageSeq { get; set; }

        public int? Age { get; set; }

        public byte? Gender { get; set; }

        public bool IsLocked { get; set; }

        public MedicalConditionsDto medicalConditions { get; set; }

        public bool readOnly { get; set; }

        public bool AutoImmune { get; set; }

        public bool? IsAdminView { get; set; }

        public bool hasInterest { get; set; }

        public bool AllergyMedBool
        {
            get { return medicalConditions != null && medicalConditions.AllergyMed == 1; }
            set { medicalConditions.AllergyMed = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool RefluxMedBool
        {
            get { return medicalConditions != null && medicalConditions.RefluxMed == 1; }
            set { medicalConditions.RefluxMed = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool UlcerMedBool
        {
            get { return medicalConditions != null && medicalConditions.UlcerMed == 1; }
            set { medicalConditions.UlcerMed = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool MigraineMedBool
        {
            get { return medicalConditions != null && medicalConditions.MigraineMed == 1; }
            set { medicalConditions.MigraineMed = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool OsteoporosisMedBool
        {
            get { return medicalConditions != null && medicalConditions.OsteoporosisMed == 1; }
            set { medicalConditions.OsteoporosisMed = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool AnxietyMedBool
        {
            get { return medicalConditions != null && medicalConditions.AnxietyMed == 1; }
            set { medicalConditions.AnxietyMed = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool DepressionMedBool
        {
            get { return medicalConditions != null && medicalConditions.DepressionMed == 1; }
            set { medicalConditions.DepressionMed = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool BackPainMedBool
        {
            get { return medicalConditions != null && medicalConditions.BackPainMed == 1; }
            set { medicalConditions.BackPainMed = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool NoPrescMedBool
        {
            get { return medicalConditions != null && medicalConditions.NoPrescMed == 1; }
            set { medicalConditions.NoPrescMed = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }
        public float hracompletion { get; set; }
		public string DTCOrgCode { get; internal set; }
	}

    public class OtherRisksModel
    {
        public int participantId { get; set; }

        public int userId { get; set; }

        public int systemAdminId { get; set; }

        public int? userinProgramId { get; set; }

        public int? participantPortalId { get; set; }

        public int? hraId { get; set; }

        public int? integrationWith { get; set; }

        public int? HRAVer { get; set; }

        public DateTime? hraCompleteDate { get; set; }

        public string hraPageSeqDone { get; set; }

        public string hraPageSeq { get; set; }

        public OtherRiskFactorsDto otherRisks { get; set; }

        public IEnumerable<SelectListItem> SmokeHistList { get; set; }

        public IEnumerable<SelectListItem> ExerciseFrequencyList { get; set; }

        public IEnumerable<SelectListItem> ExerciseIntensityList { get; set; }

        public IEnumerable<SelectListItem> ExerciseDurationList { get; set; }

        public bool readOnly { get; set; }

        public bool GINAQuestion { get; set; }

        public bool smokingProgram { get; set; }

        public bool pastHRA { get; set; }

        public string durationWeeks = "L2483";

        public string durationMonths = "L2482";

        public bool CigarBool
        {
            get { return otherRisks != null && otherRisks.Cigar == 1; }
            set { otherRisks.Cigar = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool PipeBool
        {
            get { return otherRisks != null && otherRisks.Pipe == 1; }
            set { otherRisks.Pipe = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool SmokelessTobBool
        {
            get { return otherRisks != null && otherRisks.SmokelessTob == 1; }
            set { otherRisks.SmokelessTob = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool WaterPipesBool
        {
            get { return otherRisks != null && otherRisks.WaterPipes == 1; }
            set { otherRisks.WaterPipes = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool OtherFormofTobBool
        {
            get { return otherRisks != null && otherRisks.OtherFormofTob == 1; }
            set { otherRisks.OtherFormofTob = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool FeelTiredBool
        {
            get { return otherRisks != null && otherRisks.FeelTired == 1; }
            set { otherRisks.FeelTired = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool SnoreBool
        {
            get { return otherRisks != null && otherRisks.Snore == 1; }
            set { otherRisks.Snore = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool BreathPauseBool
        {
            get { return otherRisks != null && otherRisks.BreathPause == 1; }
            set { otherRisks.BreathPause = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool HeadacheBool
        {
            get { return otherRisks != null && otherRisks.Headache == 1; }
            set { otherRisks.Headache = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool SleepyBool
        {
            get { return otherRisks != null && otherRisks.Sleepy == 1; }
            set { otherRisks.Sleepy = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool NoIssueBool
        {
            get { return otherRisks != null && otherRisks.NoIssue == 1; }
            set { otherRisks.NoIssue = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool ArthritisBool
        {
            get { return otherRisks != null && otherRisks.Arthritis == 1; }
            set { otherRisks.Arthritis = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool BreathProbBool
        {
            get { return otherRisks != null && otherRisks.BreathProb == 1; }
            set { otherRisks.BreathProb = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool BackInjuryBool
        {
            get { return otherRisks != null && otherRisks.BackInjury == 1; }
            set { otherRisks.BackInjury = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool ChronicPainBool
        {
            get { return otherRisks != null && otherRisks.ChronicPain == 1; }
            set { otherRisks.ChronicPain = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool OtherPhysLimitBool
        {
            get { return otherRisks != null && otherRisks.OtherPhysLimit == 1; }
            set { otherRisks.OtherPhysLimit = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool WalkingBool
        {
            get { return otherRisks != null && otherRisks.Walking == 1; }
            set { otherRisks.Walking = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool JoggingBool
        {
            get { return otherRisks != null && otherRisks.Jogging == 1; }
            set { otherRisks.Jogging = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool TreadmillBool
        {
            get { return otherRisks != null && otherRisks.Treadmill == 1; }
            set { otherRisks.Treadmill = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool CyclingBool
        {
            get { return otherRisks != null && otherRisks.Cycling == 1; }
            set { otherRisks.Cycling = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool StairMachBool
        {
            get { return otherRisks != null && otherRisks.StairMach == 1; }
            set { otherRisks.StairMach = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool EllipticTrainerBool
        {
            get { return otherRisks != null && otherRisks.EllipticTrainer == 1; }
            set { otherRisks.EllipticTrainer = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool RowingMachBool
        {
            get { return otherRisks != null && otherRisks.RowingMach == 1; }
            set { otherRisks.RowingMach = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool AerobicMachBool
        {
            get { return otherRisks != null && otherRisks.AerobicMach == 1; }
            set { otherRisks.AerobicMach = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool AerobicDanceBool
        {
            get { return otherRisks != null && otherRisks.AerobicDance == 1; }
            set { otherRisks.AerobicDance = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool OutdoorCyclingBool
        {
            get { return otherRisks != null && otherRisks.OutdoorCycling == 1; }
            set { otherRisks.OutdoorCycling = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool SwimmingLapsBool
        {
            get { return otherRisks != null && otherRisks.SwimmingLaps == 1; }
            set { otherRisks.SwimmingLaps = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool RacquetSportsBool
        {
            get { return otherRisks != null && otherRisks.RacquetSports == 1; }
            set { otherRisks.RacquetSports = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool OtherAerobicBool
        {
            get { return otherRisks != null && otherRisks.OtherAerobic == 1; }
            set { otherRisks.OtherAerobic = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

        public bool NoAerobicBool
        {
            get { return otherRisks != null && otherRisks.NoAerobic == 1; }
            set { otherRisks.NoAerobic = value ? Convert.ToByte(1) : Convert.ToByte(2); }
        }

		public string DTCOrgCode { get; set; }
	}

    public class ExamsModel
    {
        public int participantId { get; set; }

        public int userId { get; set; }

        public int systemAdminId { get; set; }

        public int? userinProgramId { get; set; }

        public int? participantPortalId { get; set; }

        public int? hraId { get; set; }

        public int? integrationWith { get; set; }

        public DateTime? hraCompleteDate { get; set; }

        public string hraPageSeqDone { get; set; }

        public string hraPageSeq { get; set; }

        public bool PhysicalExamBool { get; set; }

        public bool StoolTestBool { get; set; }

        public bool ColTestBool { get; set; }

        public bool SigTestBool { get; set; }

        public bool ColStoolTestBool { get; set; }

        public bool PSATestBool { get; set; }

        public bool PapTestBool { get; set; }

        public bool BoneTestBool { get; set; }

        public bool MammogramBool { get; set; }

        public bool DentalExamBool { get; set; }

        public bool BPCheckBool { get; set; }

        public bool CholTestBool { get; set; }

        public bool GlucoseTestBool { get; set; }

        public bool EyeExamBool { get; set; }

        public bool NoTestBool { get; set; }

        public bool TetanusShotBool { get; set; }

        public bool FluShotBool { get; set; }

        public bool MMRBool { get; set; }

        public bool VaricellaBool { get; set; }

        public bool HepBShotBool { get; set; }

        public bool ShinglesShotBool { get; set; }

        public bool HPVShotBool { get; set; }

        public bool PneumoniaShotBool { get; set; }

        public bool NoShotsBool { get; set; }

        public byte? Gender { get; set; }

        public bool readOnly { get; set; }
		public string DTCOrgCode { get; set; }
	}

    public class InterestModel
    {
        public int participantId { get; set; }

        public int userId { get; set; }

        public int systemAdminId { get; set; }

        public int? userinProgramId { get; set; }

        public int? participantPortalId { get; set; }

        public int? hraId { get; set; }

        public int? integrationWith { get; set; }

        public DateTime? hraCompleteDate { get; set; }

        public string hraPageSeqDone { get; set; }

        public string hraPageSeq { get; set; }

        public InterestsDto interests { get; set; }

        public byte? isSmoker { get; set; }

        public bool readOnly { get; set; }

        public byte? isPregnant { get; set; }
		public string DTCOrgCode { get; set; }
	}

    public class HealthNumbersModel
    {
        public int participantId { get; set; }

        public int userId { get; set; }

        public int OrganizationId { get; set; }

        public int southUniversityOrgId { get; set; }

        public int systemAdminId { get; set; }

        public int? userinProgramId { get; set; }

        public string DateFormat { get; set; }

        public int? Unit { get; set; }

        public int? participantPortalId { get; set; }

        public int? hraId { get; set; }

        public int? integrationWith { get; set; }

        public bool IsParticipantView { get; set; }

        public int? HRAVer { get; set; }

        public DateTime? hraCompleteDate { get; set; }

        public string hraPageSeqDone { get; set; }

        public string hraPageSeq { get; set; }

        public HealthNumbersDto HealthNumbers { get; set; }

        public HealthNumbersDto HealthNumbersInMetric { get; set; }

        public string BloodTestDate { get; set; }

        public float? HeightFt { get; set; }

        public float? HeightInch { get; set; }

        public byte? LabSelection { get; set; }

        public DateTime? LabCompleteDate { get; set; }

        public DateTime? LabBloodTestDate { get; set; }

        public string LabOrder { get; set; }

        public string OrderNo { get; set; }

        public bool hasRecentLab { get; set; }

        public bool IsAdmin { get; set; }

        public int? LabId { get; set; }

        public bool readOnly { get; set; }

        public bool readOnlyLab { get; set; }

        public bool CACScanQuestion { get; set; }

        public bool LabIntegration { get; set; }

        public bool usePreviousLabs { get; set; }

        public string ValidLabs { get; set; }

        public IList<MeasurementsDto> Measurements { get; set; }

        public IList<MeasurementsDto> MetricMeasurements { get; set; }

        public bool recentLabs { get; set; }

        public string incentive { get; set; }

        public string UserName { get; set; }

        public bool IncompleteHRA { get; set; }

        public int? DiagnosticLabId { get; set; }

        public bool ShowLabDetails { get; set; }

        public IEnumerable<SelectListItem> LabList { get; set; }
		public string DTCOrgCode { get; set; }
	}
    public class HSPModel
    {
        public int participantId { get; set; }

        public int userId { get; set; }

        public int systemAdminId { get; set; }

        public int? userinProgramId { get; set; }

        public int? participantPortalId { get; set; }

        public int? hraId { get; set; }

        public int? HRAVer { get; set; }

        public int? integrationWith { get; set; }

        public DateTime? hraCompleteDate { get; set; }

        public string hraPageSeqDone { get; set; }

        public string hraPageSeq { get; set; }

        public HSPDto hsp { get; set; }

        public IEnumerable<SelectListItem> StateOfHealthList { get; set; }

        public IEnumerable<SelectListItem> LifeSatisfactionList { get; set; }

        public IEnumerable<SelectListItem> JobSatisfactionList { get; set; }

        public IEnumerable<SelectListItem> RelaxMedList { get; set; }

        public IEnumerable<SelectListItem> WorkMissPersList { get; set; }

        public IEnumerable<SelectListItem> WorkMissFamList { get; set; }

        public IEnumerable<SelectListItem> EmergRoomVisitList { get; set; }

        public IEnumerable<SelectListItem> AdmitHospList { get; set; }

        public IEnumerable<SelectListItem> DrVisitPersList { get; set; }

        public IEnumerable<SelectListItem> ProductivityLossList { get; set; }

        public bool readOnly { get; set; }
		public string DTCOrgCode { get; set; }
	}

    public class GenericIdText
    {
        public int? Id { get; set; }

        public string Text { get; set; }
    }

    public class HRAModuleModel
    {
        public HRASummaryModel HRASummary { get; set; }

        public int hraid { get; set; }
    }
}