namespace Intervent.Web.DTO
{
    public class EXT_NutritionDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime Timestamp { get; set; }

        public string Utc_offset { get; set; }

        public double? Calories { get; set; }

        public double? Carbohydrates { get; set; }

        public double? Fat { get; set; }

        public double? Fiber { get; set; }

        public double? Protein { get; set; }

        public double? Sodium { get; set; }

        public double? Water { get; set; }

        public string Meal { get; set; }

        public string Source { get; set; }

        public string Name { get; set; }

        public string Last_updated { get; set; }

        public bool? Validated { get; set; }

        public string ExternalId { get; set; }

        public int ReadingId { get; set; }

        public double Servings { get; set; }

        public string ServingUnit { get; set; }

        public double ServingSize { get; set; }

        public double Weight { get; set; }

        public bool IsActive { get; set; }

        public string Inputmethod { get; set; }

    }
}
