namespace Intervent.Business
{
    public class EXT_BloodPressureRequest
    {
        public int id { get; set; }

        public int user_id { get; set; }

        public DateTime timestamp { get; set; }

        public int utc_offset { get; set; }

        public int resting_heart_rate { get; set; }

        public int systolic { get; set; }

        public int diastolic { get; set; }

        public DateTime created_at { get; set; }

        public DateTime updated_at { get; set; }

        public DateTime deleted_at { get; set; }

        public string input_method { get; set; }

        public string source { get; set; }
    }
}
