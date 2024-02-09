namespace Intervent.Business
{
    public class EXT_SummaryRequest
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public DateTime end_timestamp { get; set; }
        public DateTime start_timestamp { get; set; }
        public int utc_offset { get; set; }
        public int steps { get; set; }
        public float calories_burned { get; set; }
        public float calories_bmr { get; set; }
        public float calories_burned_by_activity { get; set; }
        public float distance { get; set; }
        public int active_duration { get; set; }
        public float floors { get; set; }
        public float water { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string input_method { get; set; }
        public string source { get; set; }
    }
}