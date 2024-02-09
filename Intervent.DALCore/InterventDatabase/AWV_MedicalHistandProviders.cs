namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class AWV_MedicalHistandProviders
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public byte? Alcoholism { get; set; }

        public byte? Anemia { get; set; }

        public byte? Asthma { get; set; }

        public byte? Arthritis { get; set; }

        public byte? BladderProblems { get; set; }

        public byte? Blindness { get; set; }

        public byte? BloodDisorder { get; set; }

        public byte? BloodClots { get; set; }

        public byte? BreastCancer { get; set; }

        public byte? ColonCancer { get; set; }

        public byte? ColonPolyps { get; set; }

        public byte? CancerOther { get; set; }

        public byte? Cataracts { get; set; }

        public byte? Depression { get; set; }

        public byte? Diabetes { get; set; }

        public byte? COPD { get; set; }

        public byte? GERD { get; set; }

        public byte? Glaucoma { get; set; }

        public byte? HayFever { get; set; }

        public byte? HearingLoss { get; set; }

        public byte? HeartAttack { get; set; }

        public byte? HeartDisease { get; set; }

        public byte? Hepatitis { get; set; }

        public byte? HighCholesterol { get; set; }

        public byte? Jaundice { get; set; }

        public byte? Gout { get; set; }

        public byte? Hypertension { get; set; }

        public byte? HIVorAIDS { get; set; }

        public byte? KidneyDisease { get; set; }

        public byte? KidneyStones { get; set; }

        public byte? LiverDisease { get; set; }

        public byte? MentalIllness { get; set; }

        public byte? NeurologicDisease { get; set; }

        public byte? Osteoporosis { get; set; }

        public byte? Pacemaker { get; set; }

        public byte? PhysicalDisability { get; set; }

        public byte? Pneumonia { get; set; }

        public byte? RheumaticFever { get; set; }

        public byte? SeizureDisorder { get; set; }

        public byte? SleepDisorder { get; set; }

        public byte? StomachDisorder { get; set; }

        public byte? Stroke { get; set; }

        public byte? ThyroidDisease { get; set; }

        public byte? Tuberculosis { get; set; }

        public byte? Ulcer { get; set; }

        public byte? Abdominal { get; set; }

        public byte? Appendectomy { get; set; }

        public byte? BackSurgery { get; set; }

        public byte? BreastSurgery { get; set; }

        public byte? CSection { get; set; }

        public byte? Colonoscopy { get; set; }

        public byte? DandC { get; set; }

        public byte? EGD { get; set; }

        public byte? EyeSurgery { get; set; }

        public byte? Gallbladder { get; set; }

        public byte? HeartCath { get; set; }

        public byte? HeartSurgery { get; set; }

        public byte? HerniaRepair { get; set; }

        public byte? HipSurgery { get; set; }

        public byte? Hysterectomy { get; set; }

        public byte? KneeSurgery { get; set; }

        public byte? NasalSurgery { get; set; }

        public byte? NeckSurgery { get; set; }

        public byte? SinusSurgery { get; set; }

        public byte? OvarianSurgery { get; set; }

        public byte? PlasticSurgery { get; set; }

        public byte? Thyroidectomy { get; set; }

        public byte? TonsilandAdenoid { get; set; }

        public byte? TubalLigation { get; set; }

        public byte? Vasectomy { get; set; }

        public byte? ProstateSurgery { get; set; }

        [StringLength(500)]
        public string? OtherChronProbandSurg { get; set; }

        public byte? FHAlcohol { get; set; }

        public byte? FHSubsAbuse { get; set; }

        public byte? FHAlzheimers { get; set; }

        public byte? FHDementia { get; set; }

        public byte? FHBreastCancer { get; set; }

        public byte? FHProstateCancer { get; set; }

        public byte? FHOtherCancer { get; set; }

        public byte? FHDiabetes { get; set; }

        public byte? FHMentalillness { get; set; }

        public byte? FHSuicide { get; set; }

        public byte? FHHypertension { get; set; }

        public byte? FHHeartAttack { get; set; }

        public byte? FHOsteoporosis { get; set; }

        public byte? FHSeizures { get; set; }

        public byte? FHStroke { get; set; }

        public byte? FHTB { get; set; }

        public byte? FHAdopted { get; set; }

        [StringLength(500)]
        public string? Comments { get; set; }

        public byte? FHColonCancer { get; set; }

        public virtual AWV AWV { get; set; }
    }
}
