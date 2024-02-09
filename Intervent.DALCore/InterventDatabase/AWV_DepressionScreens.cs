namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class AWV_DepressionScreens
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public byte? LittleInterest { get; set; }

        public byte? FeelingDown { get; set; }

        public byte? SleepTrouble { get; set; }

        public byte? Tired { get; set; }

        public byte? EatingIssue { get; set; }

        public byte? FeelingBad { get; set; }

        public byte? TroubleConc { get; set; }

        public byte? TroubleMoving { get; set; }

        public byte? BetterDead { get; set; }

        public byte? DiffLevel { get; set; }

        public byte? PHQ9_Score { get; set; }

        public byte? PHQ9_ProviderScore { get; set; }

        public virtual AWV AWV { get; set; }
    }
}
