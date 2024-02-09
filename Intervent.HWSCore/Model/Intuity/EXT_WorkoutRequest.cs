namespace Intervent.Business
{
    public class EXT_WorkoutRequest
    {
        public int id { get; set; }

        public int user_id { get; set; }

        public DateTime start_timestamp { get; set; }

        public int utc_offset { get; set; }

        public int duration { get; set; }

        public DateTime end_timestamp { get; set; }

        public int calories_burned { get; set; }

        public int distance { get; set; }

        public string name { get; set; }

        public string category { get; set; }

        public DateTime created_at { get; set; }

        public DateTime updated_at { get; set; }

        public DateTime deleted_at { get; set; }

        public string input_method { get; set; }

        public string source { get; set; }

    }
}
