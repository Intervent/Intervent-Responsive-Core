namespace Intervent.Web.DTO
{
    public class HSPDto
    {
        public int HRAId { get; set; }

        public byte? StateOfHealth { get; set; }

        public byte? LifeSatisfaction { get; set; }

        public byte? JobSatisfaction { get; set; }

        public byte? RelaxMed { get; set; }

        public int? WorkMissPers { get; set; }

        public int? WorkMissFam { get; set; }

        public int? EmergRoomVisit { get; set; }

        public int? AdmitHosp { get; set; }

        public int? DrVisitPers { get; set; }

        public int? ProductivityLoss { get; set; }

        public byte? TextDrive { get; set; }

        public byte? DUI { get; set; }

        public byte? RideDUI { get; set; }

        public byte? RideNoBelt { get; set; }

        public byte? Speed10 { get; set; }

        public byte? BikeNoHelmet { get; set; }

        public byte? MBikeNoHelmet { get; set; }

        public byte? SmokeDetect { get; set; }

        public byte? FireExting { get; set; }

        public byte? LiftRight { get; set; }

        public HRADto HRA { get; set; }
    }
}