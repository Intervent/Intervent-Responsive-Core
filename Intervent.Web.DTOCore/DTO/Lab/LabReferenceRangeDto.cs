namespace Intervent.Web.DTO
{
    public class LabReferenceRangeDto
    {
        public string Name { get; set; }

        public string Units { get; set; }

        public float? NormalMin { get; set; }

        public float? NormalMax { get; set; }

        public float? NormalforMale { get; set; }

        public float? NormalforFemale { get; set; }

        public float? OutofRangeMin { get; set; }

        public float? OutofRangeMax { get; set; }

        public float? OutofRangeforMale { get; set; }

        public float? OutofRangeforFemale { get; set; }

        public float? AbnormalMin { get; set; }

        public float? AbnormalMax { get; set; }

        public float? CriticalLessthan { get; set; }

        public float? CriticalGreaterthan { get; set; }

        public float? CoachCalGreaterthan { get; set; }

        public float? CoachCalDM { get; set; }

        public float? Cotinine { get; set; }
    }
}
