using Intervent.Web.DataLayer;
using Intervent.Web.DTO;

namespace InterventWebApp
{
    public class RecipeUtility
    {
        public static GetRecipeListResponse ListRecipes(int page, int pageSize, int? totalRecords)
        {
            RecipeReader reader = new RecipeReader();
            ListRecipeRequest request = new ListRecipeRequest();
            request.page = page;
            request.pageSize = pageSize;
            request.totalRecords = totalRecords;
            return reader.ListRecipes(request);
        }

        public static GetRecipeListResponse GetRecipeList(int? foodGroup, int? course, int? feature, string recipeName, int page, int pageSize, int? totalRecords, string participantLanguagePreference)
        {
            RecipeReader reader = new RecipeReader();
            GetRecipeListRequest request = new GetRecipeListRequest();
            request.foodGroup = foodGroup;
            request.course = course;
            request.feature = feature;
            request.recipeName = recipeName;
            request.languageCode = participantLanguagePreference;
            request.Page = page;
            request.PageSize = pageSize;
            request.TotalRecords = totalRecords;
            return reader.GetRecipeList(request);
        }

        public static IList<TagsDto> ListTags()
        {
            RecipeReader reader = new RecipeReader();
            return reader.GetTagList();
        }

        public static IList<RecipeTagsDto> GetTagByRecipeId(int recipeId)
        {
            RecipeReader reader = new RecipeReader();
            return reader.GetTagsByRecipeId(recipeId);
        }

        public static GetRecipeDetailsResponse GetRecipeById(int id, string language)
        {
            RecipeReader reader = new RecipeReader();
            GetRecipeDetailsRequest request = new GetRecipeDetailsRequest();
            request.id = id;
            request.language = language;
            return reader.GetRecipeById(request);
        }

        public static void UpdateImageUrl(int recipeId, string imageUrl)
        {
            RecipeReader reader = new RecipeReader();
            UpdateImageUrlRequest request = new UpdateImageUrlRequest();
            request.recipeId = recipeId;
            request.imageUrl = imageUrl;
            reader.UpdateImageUrl(request);
        }

        public static RecipesDto CreateRecipe(string name, string ingredients, string direction, string yield, int createdBy)
        {
            RecipeReader reader = new RecipeReader();
            RecipesDto recipe = new RecipesDto();
            recipe.Name = name;
            recipe.Yield = yield;
            recipe.Direction = direction;
            recipe.Ingredients = ingredients;
            recipe.IsActive = true;
            recipe.CreatedBy = createdBy;
            return reader.AddEditRecipe(recipe);
        }

        public static RecipesDto EditRecipe(int id, string name, string yield, string servingsize, string carbohydrate, string fat, bool isActive, string calories, string imageUrl, string direction, string ingredients)
        {
            RecipeReader reader = new RecipeReader();
            RecipesDto recipe = new RecipesDto();
            recipe.Id = id;
            recipe.Name = name;
            recipe.Yield = yield;
            recipe.ServingSize = servingsize;
            recipe.Carbohydrate = carbohydrate;
            recipe.Fat = fat;
            recipe.Calories = calories;
            recipe.IsActive = isActive;
            recipe.ImageURL = imageUrl;
            recipe.Direction = direction;
            recipe.Ingredients = ingredients;
            return reader.AddEditRecipe(recipe);
        }

        public static bool EditRecipeTranslation(int id, string name, string yield, string servingsize, string carbohydrate, string fat, bool isActive, string calories, string imgUrl, string direction, string ingredients, string language)
        {
            RecipeReader reader = new RecipeReader();
            RecipeTranslationDto recipeTranslation = new RecipeTranslationDto();
            recipeTranslation.RecipeId = id;
            recipeTranslation.LanguageCode = language;
            recipeTranslation.Name = name;
            recipeTranslation.Yield = yield;
            recipeTranslation.ServingSize = servingsize;
            recipeTranslation.Carbohydrate = carbohydrate;
            recipeTranslation.Fat = fat;
            recipeTranslation.Calories = calories;
            recipeTranslation.IsActive = isActive;
            recipeTranslation.ImageURL = imgUrl;
            recipeTranslation.Direction = direction;
            recipeTranslation.Ingredients = ingredients;
            return reader.AddEditRecipeTranslation(recipeTranslation);
        }

        public static bool AddEditTag(int recipeId, int[] tagIds)
        {
            RecipeReader reader = new RecipeReader();
            List<RecipeTagsDto> tags = new List<RecipeTagsDto>();
            for (int i = 0; i < tagIds.Length; i++)
            {
                RecipeTagsDto tag = new RecipeTagsDto();
                tag.RecipeId = recipeId;
                tag.TagId = tagIds[i];
                tag.IsActive = true;
                tags.Add(tag);
            }
            return reader.AddEditTag(tags);
        }

        public static bool AssignRecipe(AssignRecipeModel model)
        {
            RecipeReader reader = new RecipeReader();
            AssignedRecipeRequest request = new AssignedRecipeRequest();
            List<AssignedRecipeDto> recipes = new List<AssignedRecipeDto>();
            var OrganizationIdStr = model.OrganizationIds.Split(',');

            for (int i = 0; i < OrganizationIdStr.Length; i++)
            {
                AssignedRecipeDto assignRecipe = new AssignedRecipeDto();
                assignRecipe.RecipeId = model.RecipeId;
                assignRecipe.OrganizationId = Convert.ToInt16(OrganizationIdStr[i]);
                recipes.Add(assignRecipe);
            }
            request.recipeList = recipes;
            return reader.AssignRecipe(request);
        }

        public static IList<AssignedRecipeDto> ListAssignedRecipes()
        {
            RecipeReader reader = new RecipeReader();
            return reader.ListAssignedRecipes();
        }
    }
}