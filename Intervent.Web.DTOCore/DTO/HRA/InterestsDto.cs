namespace Intervent.Web.DTO
{
    public class InterestsDto
    {
        public int HRAId { get; set; }

        public byte? WeightManProg { get; set; }

        public byte? MaternityProg { get; set; }

        public byte? ExerProg { get; set; }

        public byte? QuitSmokeProg { get; set; }

        public byte? NutProg { get; set; }

        public byte? StressManProg { get; set; }

        public byte? CompProg { get; set; }

        public HRADto HRA { get; set; }
    }
}