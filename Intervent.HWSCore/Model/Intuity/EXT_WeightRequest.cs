namespace Intervent.Business
{
    public class EXT_WeightRequest
    {
        public int id { get; set; }

        public int user_id { get; set; }

        public DateTime timestamp { get; set; }

        public int utc_offset { get; set; }

        public double weight { get; set; }

        public double bmi { get; set; }

        public double fat_percent { get; set; }

        public DateTime created_at { get; set; }

        public DateTime updated_at { get; set; }

        public DateTime deleted_at { get; set; }

        public string input_method { get; set; }

        public string source { get; set; }
    }
}
