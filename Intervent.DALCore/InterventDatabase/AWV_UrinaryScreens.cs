namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class AWV_UrinaryScreens
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public byte? WhenCough { get; set; }

        public byte? WhenBend { get; set; }

        public byte? WhenExer { get; set; }

        public byte? WhenUndress { get; set; }

        public byte? BefReachToilet { get; set; }

        public byte? RushtoBath { get; set; }

        public byte? AffHousChores { get; set; }

        public byte? AffPhyRec { get; set; }

        public byte? AffEnt { get; set; }

        public byte? Travel { get; set; }

        public byte? Social { get; set; }

        public byte? FeelFrust { get; set; }

        public byte? UrnRiskScore { get; set; }

        public virtual AWV AWV { get; set; }
    }
}
