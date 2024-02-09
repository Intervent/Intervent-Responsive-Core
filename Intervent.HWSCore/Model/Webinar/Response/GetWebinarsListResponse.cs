namespace Intervent.HWS
{
    public class GetWebinarsListResponse : ProcessResponse
    {
        public ListWebinar webinarsList { get; set; }

        public class ListWebinar
        {
            public int page_size { get; set; }
            public int total_records { get; set; }
            public string next_page_token { get; set; }
            public List<Webinar> webinars { get; set; }
        }

        public class Webinar
        {
            public string uuid { get; set; }
            public string id { get; set; }
            public string host_id { get; set; }
            public string topic { get; set; }
            public int type { get; set; }
            public DateTime start_time { get; set; }
            public int duration { get; set; }
            public string timezone { get; set; }
            public string agenda { get; set; }
            public DateTime created_at { get; set; }
            public string join_url { get; set; }
        }
    }
}
