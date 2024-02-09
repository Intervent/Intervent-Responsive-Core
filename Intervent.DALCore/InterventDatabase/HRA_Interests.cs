namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class HRA_Interests
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public byte? MaternityProg { get; set; }

        public byte? WeightManProg { get; set; }

        public byte? ExerProg { get; set; }

        public byte? QuitSmokeProg { get; set; }

        public byte? NutProg { get; set; }

        public byte? StressManProg { get; set; }

        public byte? CompProg { get; set; }

        public virtual HRA HRA { get; set; }
    }
}
