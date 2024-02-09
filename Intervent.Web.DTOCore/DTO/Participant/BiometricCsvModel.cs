namespace Intervent.Web.DTO
{
    public class BiometricCsvModel
    {

        public int UniqueId { get; set; }

        public VisitTypeDto Visit { get; set; }

        public short? SBP { get; set; }

        public short? DBP { get; set; }

        public float? Height { get; set; }

        public float? Weight { get; set; }

        public float? TotalChol { get; set; }

        public float? LDL { get; set; }

        public float? HDL { get; set; }

        public float? Trig { get; set; }

        public float? A1C { get; set; }

        public YesNoDto DidYouFast { get; set; }

        public DateTime? AssessmentDate { get; set; }

    }
}
