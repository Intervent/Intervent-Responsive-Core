namespace Intervent.Web.DTO
{
    public class ExamsandShotsDto
    {
        public int HRAId { get; set; }

        public byte? PhysicalExam { get; set; }

        public byte? StoolTest { get; set; }

        public byte? SigTest { get; set; }

        public byte? ColStoolTest { get; set; }

        public byte? ColTest { get; set; }

        public byte? PSATest { get; set; }

        public byte? PapTest { get; set; }

        public byte? BoneTest { get; set; }

        public byte? Mammogram { get; set; }

        public byte? DentalExam { get; set; }

        public byte? BPCheck { get; set; }

        public byte? CholTest { get; set; }

        public byte? GlucoseTest { get; set; }

        public byte? EyeExam { get; set; }

        public byte? NoTest { get; set; }

        public byte? TetanusShot { get; set; }

        public byte? FluShot { get; set; }

        public byte? MMR { get; set; }

        public byte? Varicella { get; set; }

        public byte? HepBShot { get; set; }

        public byte? ShinglesShot { get; set; }

        public byte? HPVShot { get; set; }

        public byte? PneumoniaShot { get; set; }

        public byte? NoShots { get; set; }

        public HRADto HRA { get; set; }
    }
}