namespace Intervent.Web.DTO
{
    public class TrackCanriskRequest
    {
        public string Guid { get; set; }

        public int pageCompleted { get; set; }

        public string utm_source { get; set; }

        public string utm_medium { get; set; }

        public string utm_campaign { get; set; }

        public string utm_keywords { get; set; }

        public string Reason { get; set; }

        public int? ReasonId { get; set; }

        public int? EligibilityId { get; set; }

        public DateTime? DOB { get; set; }

        public byte? Gender { get; set; }
    }
}
