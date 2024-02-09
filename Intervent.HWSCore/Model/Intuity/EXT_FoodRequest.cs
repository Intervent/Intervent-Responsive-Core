namespace Intervent.Business
{
    public class EXT_FoodRequest
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int reading_id { get; set; }
        public DateTime timestamp { get; set; }
        public int utc_offset { get; set; }
        public string name { get; set; }
        public double servings { get; set; }
        public string serving_unit { get; set; }
        public double serving_size { get; set; }
        public double weight { get; set; }
        public double calories { get; set; }
        public double carbohydrates { get; set; }
        public double fat { get; set; }
        public double protein { get; set; }
        public double dietary_fiber { get; set; }
        public double sodium { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime deleted_at { get; set; }
        public string input_method { get; set; }
        public string source { get; set; }
    }
}