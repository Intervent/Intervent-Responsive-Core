namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class AWV_OsteoporosisScreens
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public byte? Osteoporosis { get; set; }

        public byte? Olderthan65 { get; set; }

        public byte? BoneDensitometry { get; set; }

        public byte? OnCartisone { get; set; }

        public byte? SmallBoned { get; set; }

        public byte? DrinkorSmoke { get; set; }

        public byte? FHOsteoporosis { get; set; }

        public byte? InactiveLife { get; set; }

        public byte? BloodThinMed { get; set; }

        public byte? CancerMed { get; set; }

        public byte? RheumatismMed { get; set; }

        public byte? SeizureMed { get; set; }

        public byte? WaterPill { get; set; }

        public byte? ThyroidMed { get; set; }

        public byte? OstMed { get; set; }

        public byte? Alcohololism { get; set; }

        public byte? KidStone { get; set; }

        public byte? Cancer { get; set; }

        public byte? Malabsorption { get; set; }

        public byte? EastDis { get; set; }

        public byte? Rheumatism { get; set; }

        public byte? HypThy { get; set; }

        public byte? ThyDis { get; set; }

        public byte? Backache { get; set; }

        public byte? Posture { get; set; }

        public byte? LostHeight { get; set; }

        public byte? CompFrac { get; set; }

        public byte? HipRep { get; set; }

        public byte? BrokenBones { get; set; }

        public byte? PostMeno { get; set; }

        public byte? PerBef45 { get; set; }

        public byte? OvRem { get; set; }

        public byte? HotFlash { get; set; }

        public byte? OnEstogen { get; set; }

        public byte? OstRiskScore { get; set; }

        public virtual AWV AWV { get; set; }
    }
}
