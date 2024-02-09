using Intervent.Web.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class RecipeController : BaseController
    {
        #region Manage Recipes

        [ModuleControl(Modules.Recipes)]
        public ActionResult RecipeList()
        {
            return View();
        }

        [ModuleControl(Modules.Recipes)]
        public JsonResult ListRecipes(int page, int pageSize, int? totalRecords)
        {
            var response = RecipeUtility.ListRecipes(page, pageSize, totalRecords);
            return Json(new { Result = "OK", Records = response.recipes, TotalRecords = response.totalRecords });
        }

        [ModuleControl(Modules.Recipes)]
        public ActionResult RecipeDetails(int id)
        {
            RecipeDetailModel model = new RecipeDetailModel();
            model.Id = id;
            model.RecipeTags = RecipeUtility.ListTags();
            model.SelectedTags = RecipeUtility.GetTagByRecipeId(id);
            ViewData["languageList"] = CommonUtility.GetLanguages().Languages.Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.LanguageCode });
            return View(model);
        }

        [HttpPost]
        public JsonResult GetRecipeDetails(int id, string language)
        {
            var response = RecipeUtility.GetRecipeById(id, language);
            return Json(new { Result = "OK", Record = response.recipeDetail });
        }

        [ModuleControl(Modules.Recipes)]
        [HttpPost]
        public JsonResult CreateRecipe(string name, string ingredients, string direction, string serving)
        {
            var createdBy = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            var response = RecipeUtility.CreateRecipe(name, ingredients, direction, serving, createdBy);

            return Json(new { Result = "OK", Record = response.Id });
        }

        [ModuleControl(Modules.Recipes)]
        [HttpPost]
        public JsonResult EditRecipe(int id, string name, string yield, string servingsize, string carbohydrate, string fat, bool isActive, string calories, string imageUrl, string direction, string ingredients, string language)
        {
            if (!string.IsNullOrEmpty(language) && language != ListOptions.DefaultLanguage)
            {

                var response = RecipeUtility.EditRecipeTranslation(id, name, yield, servingsize, carbohydrate, fat, isActive, calories, imageUrl, direction, ingredients, language);
                return Json(new { Result = "OK", Status = response });
            }
            else
            {
                var response = RecipeUtility.EditRecipe(id, name, yield, servingsize, carbohydrate, fat, isActive, calories, imageUrl, direction, ingredients);

                return Json(new { Result = "OK", Record = response.Id });
            }
        }

        [ModuleControl(Modules.Recipes)]
        [HttpPost]
        public JsonResult AddEditTag(int recipeId, int[] tagIds)
        {
            var response = RecipeUtility.AddEditTag(recipeId, tagIds);

            return Json(new { Result = "OK", Record = RecipeUtility.GetTagByRecipeId(recipeId) });
        }

        [ModuleControl(Modules.Recipes)]
        [HttpPost]
        public JsonResult AssignRecipe([FromBody] AssignRecipeModel model)
        {
            var response = RecipeUtility.AssignRecipe(model);
            return Json(new { Result = "OK", Record = response });
        }

        [ModuleControl(Modules.Recipes)]
        public JsonResult ListAssignedRecipes(int recipeId)
        {
            var result = RecipeUtility.ListAssignedRecipes();
            if (result != null && result.Count > 0 && result.Where(r => r.RecipeId == recipeId).Count() > 0)
            {
                var recipes = result.Where(r => r.RecipeId == recipeId).ToList();
                return Json(new
                {
                    Result = "OK",
                    Records = recipes.Select(x => new
                    {
                        Name = x.Organization.Name,
                        Date = x.Date,
                        Completed = x.Completed == true ? "Yes" : "No"
                    }),
                });
            }
            return Json(new { Result = "OK" });
        }

        #endregion

        #region Recipes for Users

        [Authorize]
        public ActionResult Recipes()
        {
            RecipeModel model = new RecipeModel();
            model.foodGroups = RecipeUtility.ListTags().Where(x => x.TagTypeId == 1).Select(x => new SelectListItem { Text = Translate.Message(x.LanguageCode), Value = x.Id.ToString() });
            model.courses = RecipeUtility.ListTags().Where(x => x.TagTypeId == 2).Select(x => new SelectListItem { Text = Translate.Message(x.LanguageCode), Value = x.Id.ToString() });
            model.features = RecipeUtility.ListTags().Where(x => x.TagTypeId == 3).Select(x => new SelectListItem { Text = Translate.Message(x.LanguageCode), Value = x.Id.ToString() });
            return View(model);
        }

        [Authorize]
        public JsonResult SearchRecipes(string recipeName, int? foodGroup, int? course, int? feature, int page, int pageSize, int? totalRecords)
        {
            RecipeModel model = new RecipeModel();
            return Json(new { Result = "OK", Record = RecipeUtility.GetRecipeList(foodGroup, course, feature, recipeName, page, pageSize, totalRecords, HttpContext.Session.GetString(SessionContext.ParticipantLanguagePreference)) });
        }

        [Authorize]
        public ActionResult Recipe(int id)
        {
            var recipe = RecipeUtility.GetRecipeById(id, HttpContext.Session.GetString(SessionContext.ParticipantLanguagePreference));
            return PartialView(recipe.recipeDetail);
        }
        #endregion

    }
}