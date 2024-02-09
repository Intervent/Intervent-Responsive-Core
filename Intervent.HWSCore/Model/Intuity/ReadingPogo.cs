namespace Intervent.HWS
{
    public class ReadingPogoRequest
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int pogo_id { get; set; }
        public int sequence_number { get; set; }
        public DateTime timestamp { get; set; }
        public int utc_offset { get; set; }
        public int blood_glucose { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public object last_sync_time { get; set; }
        public object auxiliary_info { get; set; }
        public string input_method { get; set; }
        public string source { get; set; }
        public string serial_number { get; set; }
        public Target target { get; set; }
        public Tag[] tags { get; set; }
    }

    public class Target
    {
        public int user_id { get; set; }
        public int hypo_limit { get; set; }
        public int target_low { get; set; }
        public int target_high { get; set; }
        public int hyper_limit { get; set; }
        public int pre_meal_low { get; set; }
        public int pre_meal_high { get; set; }
        public int post_meal_low { get; set; }
        public int post_meal_high { get; set; }
        public float e_a1c_target { get; set; }
        public Testing_Times[] testing_times { get; set; }
    }

    public class Testing_Times
    {
        public string icon { get; set; }
        public string label { get; set; }
        public bool enabled { get; set; }
        public string end_time { get; set; }
        public string start_time { get; set; }
    }

    public class Tag
    {
        public int user_id { get; set; }
        public int reading_id { get; set; }
        public string value { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string tag_type { get; set; }
        public string unit { get; set; }
        public string source { get; set; }
        public string input_method { get; set; }
    }

}