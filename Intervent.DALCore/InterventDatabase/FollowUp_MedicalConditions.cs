namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class FollowUp_MedicalConditions
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public byte? Stroke { get; set; }

        public byte? HeartAttack { get; set; }

        public byte? Angina { get; set; }

        public byte? ToldArteries { get; set; }

        public byte? ToldBlock { get; set; }

        public byte? ToldDiabetes { get; set; }

        public byte? ToldKidneyDisease { get; set; }

        public byte? HighBPMed { get; set; }

        public byte? HighCholMed { get; set; }

        public byte? DiabetesMed { get; set; }

        public byte? Insulin { get; set; }

        public byte? HeartFailMed { get; set; }

        public byte? HeartCondMed { get; set; }

        public byte? BloodThinMed { get; set; }

        public byte? AnginaMed { get; set; }

        public virtual FollowUp FollowUp { get; set; }
    }
}
