namespace Intervent.DAL
{
    public class RecipeTranslation
    {
        public int RecipeId { get; set; }

        public string LanguageCode { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public int CreatedBy { get; set; }

        public DateTime DateCreated { get; set; }

        public string? ImageURL { get; set; }

        public bool IsActive { get; set; }

        public string? Carbohydrate { get; set; }

        public string? Fat { get; set; }

        public int? Sodium { get; set; }

        public int? Sugar { get; set; }

        public string? Calories { get; set; }

        public int? Cholesterol { get; set; }

        public string Direction { get; set; }

        public string? Ingredients { get; set; }

        public string? Yield { get; set; }

        public string? ServingSize { get; set; }

        public virtual Recipe Recipe { get; set; }

    }
}
