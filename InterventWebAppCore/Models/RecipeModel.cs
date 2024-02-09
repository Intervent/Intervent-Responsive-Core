using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class RecipeModel
    {
        public IList<RecipesDto> recipes { get; set; }

        public IEnumerable<SelectListItem> foodGroups { get; set; }

        public int foodGroup { get; set; }

        public IEnumerable<SelectListItem> courses { get; set; }

        public int course { get; set; }

        public IEnumerable<SelectListItem> features { get; set; }

        public int feature { get; set; }
    }

    public class AssignRecipeModel
    {
        public int RecipeId { get; set; }

        public string OrganizationIds { get; set; }
    }

    public class RecipeDetailModel
    {
        public int Id { get; set; }

        public IList<TagsDto> RecipeTags { get; set; }

        public IList<RecipeTagsDto> SelectedTags { get; set; }

    }
}