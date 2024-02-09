namespace Intervent.Business
{
    public class EXT_SleepRequest
    {
        public int id { get; set; }

        public int user_id { get; set; }

        public DateTime start_timestamp { get; set; }

        public int utc_offset { get; set; }

        public int total_sleep_duration { get; set; }

        public int awake_duration { get; set; }

        public int awake_count { get; set; }

        public int deep_duration { get; set; }

        public int light_duration { get; set; }

        public int rem_duration { get; set; }

        public DateTime created_at { get; set; }

        public DateTime updated_at { get; set; }

        public DateTime deleted_at { get; set; }

        public string input_method { get; set; }

        public string source { get; set; }
    }
}
