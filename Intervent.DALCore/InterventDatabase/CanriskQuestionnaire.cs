using System.ComponentModel.DataAnnotations;

namespace Intervent.DAL
{
    public partial class CanriskQuestionnaire
    {
        [Key]
        public int Id { get; set; }

        public int EligibilityId { get; set; }

        public byte? PreDiabetes { get; set; }

        public byte? GlucoseLowering { get; set; }

        public byte? WeightLoss { get; set; }

        public byte? KnowA1c { get; set; }

        public float? A1c { get; set; }

        public byte? KnowGlucose { get; set; }

        public float? Glucose { get; set; }

        public byte? Diabetes { get; set; }

        public byte? SteroidMed { get; set; }

        public byte? Pancreatitis { get; set; }

        public byte? Cancer { get; set; }

        public byte? Hemoglobinopathy { get; set; }

        public byte? StudyDrug { get; set; }

        public byte? HasDementia { get; set; }

        public byte? AbilitytoRead { get; set; }

        public byte? HasSupporttoRead { get; set; }

        public byte? Pregnant { get; set; }

        public float? Weight { get; set; }

        public float? Height { get; set; }

        public float? Waist { get; set; }

        public byte? BriskWalking { get; set; }

        public byte? FruitsandVeggies { get; set; }

        public byte? BloodPressure { get; set; }

        public byte? BloodGlucose { get; set; }

        public byte? GiveBirth { get; set; }

        public byte? MothersEthnic { get; set; }

        public byte? FathersEthnic { get; set; }

        public byte? DiabetesMother { get; set; }

        public byte? DiabetesFather { get; set; }

        public byte? DiabetesSiblings { get; set; }

        public byte? DiabetesChildren { get; set; }

        public byte? DiabetesOthers { get; set; }

        public byte? DiabetesNotSure { get; set; }

        public byte? Education { get; set; }

        public bool? SenttoVendor { get; set; }

        public int? CanriskScore { get; set; }

        public bool? isEligible { get; set; }

        public bool? SentforOutreach { get; set; }

        public DateTime? CompletedOn { get; set; }

        public int? CompletedBy { get; set; }

        public string? utm_source { get; set; }

        public string? utm_medium { get; set; }

        public string? utm_campaign { get; set; }

        public string? utm_keywords { get; set; }

        public virtual Eligibility Eligibility { get; set; }

        public virtual User user { get; set; }

    }
}
