using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class FU_MedicalConditionsModel
    {
        public FollowUp_MedicalConditionsDto MedicalConditions { get; set; }

        public bool readOnly { get; set; }
    }

    public class FU_HSAModel
    {
        public IEnumerable<SelectListItem> StateOfHealthList { get; set; }

        public IEnumerable<SelectListItem> ProductivityLossList { get; set; }

        public FollowUp_HealthConditionsDto HealthCondition { get; set; }

        public bool readOnly { get; set; }

        public int? HRAVer { get; set; }
    }

    public class FU_OtherRisksModel
    {
        public IEnumerable<SelectListItem> SmokeHistList { get; set; }

        public FollowUp_OtherRiskFactorsDto otherRisks { get; set; }

        public OtherRiskFactorsDto HRA_otherRisks { get; set; }

        public bool readOnly { get; set; }

        public bool GINAQuestion { get; set; }

        public int? integrationWith { get; set; }

        public int? HRAVer { get; set; }

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
    }

    public class FU_HealthNumbersModel
    {
        public FollowUp_HealthNumbersDto HealthNumbers { get; set; }

        public FollowUp_HealthNumbersDto HealthNumbersInMetric { get; set; }

        public HealthNumbersDto PreviousHealthNumbers { get; set; }

        public bool readOnly { get; set; }

        public IList<MeasurementsDto> Measurements { get; set; }

        public IList<MeasurementsDto> MetricMeasurements { get; set; }

        public string BloodTestDate { get; set; }

        public float? HeightFt { get; set; }

        public float? HeightInch { get; set; }

        public byte? LabSelection { get; set; }

        public bool InitialFollowUp { get; set; }

        public DateTime? LabCompleteDate { get; set; }

        public DateTime? LabBloodTestDate { get; set; }

        public string LabOrder { get; set; }

        public string OrderNo { get; set; }

        public bool IsAdmin { get; set; }

        public int? LabId { get; set; }

        public int? DiagnosticLabId { get; set; }

        public bool readOnlyLab { get; set; }

        public bool LabIntegration { get; set; }

        public bool IncompleteFollowUp { get; set; }

        public bool IsParticipantView { get; set; }

        public int? HRAVer { get; set; }

        public int? Unit { get; set; }

        public string DateFormat { get; set; }

        public IEnumerable<SelectListItem> LabList { get; set; }
    }
    public class FollowUpModuleModel
    {
        public FollowupReport FollowUpSummary { get; set; }

        public int FollowUpId { get; set; }

        public string DateFormat { get; set; }
    }
}