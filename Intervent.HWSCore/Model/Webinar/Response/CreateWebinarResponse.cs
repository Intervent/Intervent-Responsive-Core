namespace Intervent.HWS
{
    public class CreateWebinarResponse : ProcessResponse
    {
        public string host_email { get; set; }

        public string host_id { get; set; }

        public long id { get; set; }

        public bool registrants_confirmation_email { get; set; }

        public string template_id { get; set; }

        public string uuid { get; set; }

        public string agenda { get; set; }

        public DateTime created_at { get; set; }

        public int duration { get; set; }

        public string join_url { get; set; }

        public List<Occurrence> occurrences { get; set; }

        public string password { get; set; }

        public Recurrence recurrence { get; set; }

        public Settings settings { get; set; }

        public DateTime start_time { get; set; }

        public string start_url { get; set; }

        public string timezone { get; set; }

        public string topic { get; set; }

        public List<TrackingField> tracking_fields { get; set; }

        public int type { get; set; }
    }

    public class Occurrence
    {
        public int duration { get; set; }

        public string occurrence_id { get; set; }

        public DateTime start_time { get; set; }

        public string status { get; set; }
    }
}
