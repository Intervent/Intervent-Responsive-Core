namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class AWV_General
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public byte? HealthRating { get; set; }

        public byte? HealthStatus { get; set; }

        public byte? TroubleTakingMeds { get; set; }

        public byte? BotheredbyDizzy { get; set; }

        public byte? BotheredbySex { get; set; }

        public byte? BotheredbyEating { get; set; }

        public byte? BotheredbyTeeth { get; set; }

        public byte? BotheredbyTelephone { get; set; }

        public byte? BotheredbyTiredness { get; set; }

        public byte? UseHearingAids { get; set; }

        public byte? FeelingDown { get; set; }

        public byte? LittleInterest { get; set; }

        [StringLength(255)]
        public string? GoToHelp { get; set; }

        public byte? BreakfastDays { get; set; }

        public byte? NoOfSweetDrinks { get; set; }

        public byte? FruitandVegServ { get; set; }

        public byte? EatWithFamilyDays { get; set; }

        [Column("20MinExerFor3Days")]
        public byte? C20MinExerFor3Days { get; set; }

        public byte? ExerIntensity { get; set; }

        public byte? BodilyPain { get; set; }

        public byte? WalkWithoutHelp { get; set; }

        public byte? DoThingsWithoutHelp { get; set; }

        public byte? NeedHelpWithBasics { get; set; }

        public byte? MemoryProblems { get; set; }

        public byte? UrineProblems { get; set; }

        public byte? Fallen2Times { get; set; }

        public byte? AfraidOfFalling { get; set; }

        public byte? SafetyIssuesAtHome { get; set; }

        public byte? FastenSeatbelt { get; set; }

        public byte? UsedTob100Times { get; set; }

        public byte? TobInLast4Weeks { get; set; }

        public byte? CaffeinatedBev { get; set; }

        public byte? ProbWithAlcohol { get; set; }

        public byte? NoOfDrinks { get; set; }

        public byte? AbusedDrugs { get; set; }

        [StringLength(500)]
        public string? Comments { get; set; }

        public virtual AWV AWV { get; set; }
    }
}
