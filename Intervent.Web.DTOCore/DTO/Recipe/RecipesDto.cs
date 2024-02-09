namespace Intervent.Web.DTO
{
    public class RecipesDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Yield { get; set; }

        public string ServingSize { get; set; }

        public int CreatedBy { get; set; }

        public string ImageURL { get; set; }

        public bool IsActive { get; set; }

        public string Carbohydrate { get; set; }

        public string Fat { get; set; }

        public Nullable<int> Sodium { get; set; }

        public Nullable<int> Sugar { get; set; }

        public string Calories { get; set; }

        public Nullable<int> Cholesterol { get; set; }

        public string Direction { get; set; }

        public string Ingredients { get; set; }

        public IList<RecipeTagsDto> RecipeTags { get; set; }

        public IList<TagsDto> TagList { get; set; }

        public virtual IList<RecipeTranslationDto> RecipeTranslations { get; set; }
    }
}
