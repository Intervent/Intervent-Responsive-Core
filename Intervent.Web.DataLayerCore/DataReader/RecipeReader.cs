using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Intervent.Web.DataLayer
{
    public class RecipeReader
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public GetRecipeListResponse ListRecipes(ListRecipeRequest request)
        {
            GetRecipeListResponse response = new GetRecipeListResponse();
            var totalRecords = request.totalRecords.HasValue ? request.totalRecords.Value : 0;
            if (totalRecords == 0)
            {
                totalRecords = context.Recipes.Count();
            }
            var recipes = context.Recipes.OrderBy(x => x.Name).Skip(request.page * request.pageSize).Take(request.pageSize).ToList();
            var RecipeList = Utility.mapper.Map<IList<DAL.Recipe>, IList<RecipesDto>>(recipes);
            response.recipes = RecipeList;
            response.totalRecords = totalRecords;
            return response;
        }

        public IList<TagsDto> GetTagList()
        {
            var tags = context.Tags.ToList();
            var list = Utility.mapper.Map<IList<DAL.Tag>, IList<TagsDto>>(tags);
            return list;
        }

        public GetRecipeDetailsResponse GetRecipeById(GetRecipeDetailsRequest request)
        {
            GetRecipeDetailsResponse response = new GetRecipeDetailsResponse();
            var recipe = context.Recipes.Include("RecipeTranslations").Where(x => x.Id == request.id).FirstOrDefault();
            response.recipeDetail = new RecipesDto();
            if (recipe != null)
            {
                var recipetranslation = recipe.RecipeTranslations.Where(x => x.LanguageCode == request.language).FirstOrDefault();
                if (!string.IsNullOrEmpty(request.language) && request.language != ListOptions.DefaultLanguage && recipetranslation != null)
                {
                    response.recipeDetail.Name = recipetranslation.Name;
                    response.recipeDetail.Calories = recipetranslation.Calories;
                    response.recipeDetail.Carbohydrate = recipetranslation.Carbohydrate;
                    response.recipeDetail.Cholesterol = recipetranslation.Cholesterol;
                    response.recipeDetail.Direction = recipetranslation.Direction;
                    response.recipeDetail.Fat = recipetranslation.Fat;
                    response.recipeDetail.Id = recipetranslation.RecipeId;
                    response.recipeDetail.Ingredients = recipetranslation.Ingredients;
                    response.recipeDetail.ServingSize = recipetranslation.ServingSize;
                    response.recipeDetail.Yield = recipetranslation.Yield;
                    response.recipeDetail.ImageURL = recipe.ImageURL;
                    response.recipeDetail.IsActive = recipe.IsActive;
                }
                else
                {
                    response.recipeDetail.Name = recipe.Name;
                    response.recipeDetail.Calories = recipe.Calories;
                    response.recipeDetail.Carbohydrate = recipe.Carbohydrate;
                    response.recipeDetail.Cholesterol = recipe.Cholesterol;
                    response.recipeDetail.Direction = recipe.Direction;
                    response.recipeDetail.Fat = recipe.Fat;
                    response.recipeDetail.Id = recipe.Id;
                    response.recipeDetail.Ingredients = recipe.Ingredients;
                    response.recipeDetail.ServingSize = recipe.ServingSize;
                    response.recipeDetail.Yield = recipe.Yield;
                    response.recipeDetail.ImageURL = recipe.ImageURL;
                    response.recipeDetail.IsActive = recipe.IsActive;
                }
            }
            return response;
        }

        public void UpdateImageUrl(UpdateImageUrlRequest request)
        {
            var recipeDAL = context.Recipes.Where(x => x.Id == request.recipeId).FirstOrDefault();
            recipeDAL.ImageURL = request.imageUrl;
            context.Recipes.Attach(recipeDAL);
            context.Entry(recipeDAL).State = EntityState.Modified;
            context.SaveChanges();
        }

        public GetRecipeListResponse GetRecipeList(GetRecipeListRequest request)
        {
            GetRecipeListResponse response = new GetRecipeListResponse();
            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            if (totalRecords == 0)
            {
                totalRecords = context.Recipes.Include("RecipeTags").Include("RecipeTranslations")
                    .Where(x => (String.IsNullOrEmpty(request.recipeName) || x.Name.Contains(request.recipeName)) && x.IsActive && x.RecipeTags.Any(y => (!request.foodGroup.HasValue || y.TagId == request.foodGroup.Value))
                        && x.RecipeTags.Any(y => (!request.course.HasValue || y.TagId == request.course.Value)) && x.RecipeTags.Any(y => (!request.feature.HasValue || y.TagId == request.feature.Value))).Count();
            }
            var recipesDAL = context.Recipes.Include("RecipeTags").Include("RecipeTranslations")
                    .Where(x => (String.IsNullOrEmpty(request.recipeName) || x.Name.Contains(request.recipeName)) && x.IsActive && x.RecipeTags.Any(y => (!request.foodGroup.HasValue || y.TagId == request.foodGroup.Value))
                        && x.RecipeTags.Any(y => (!request.course.HasValue || y.TagId == request.course.Value)) && x.RecipeTags.Any(y => (!request.feature.HasValue || y.TagId == request.feature.Value)))
                        .OrderBy(x => x.Id).Skip(request.Page * request.PageSize).Take(request.PageSize).ToList();
            if (recipesDAL != null)
            {
                List<RecipesDto> recipesDto = new List<RecipesDto>();
                for (int i = 0; i < recipesDAL.Count; i++)
                {
                    RecipesDto recipeDto = new RecipesDto();
                    var translatedRecipe = recipesDAL[i].RecipeTranslations.Where(x => x.LanguageCode == request.languageCode).FirstOrDefault();
                    if (translatedRecipe != null)
                    {
                        recipeDto.Calories = translatedRecipe.Calories;
                        recipeDto.Yield = translatedRecipe.Yield;
                        recipeDto.Name = translatedRecipe.Name;
                        recipeDto.Id = translatedRecipe.RecipeId;
                        recipeDto.ImageURL = translatedRecipe.ImageURL;
                        recipesDto.Add(recipeDto);
                    }
                    else
                    {
                        recipeDto.Calories = recipesDAL[i].Calories;
                        recipeDto.Yield = recipesDAL[i].Yield;
                        recipeDto.Name = recipesDAL[i].Name;
                        recipeDto.Id = recipesDAL[i].Id;
                        recipeDto.ImageURL = recipesDAL[i].ImageURL;
                        recipesDto.Add(recipeDto);
                    }
                }
                response.recipes = recipesDto;
            }
            response.totalRecords = totalRecords;
            return response;
        }

        public IList<RecipeTagsDto> GetTagsByRecipeId(int recipeId)
        {
            var recipeDAL = context.RecipeTags.Include("Tag").Where(x => x.RecipeId == recipeId).ToList();
            return Utility.mapper.Map<IList<DAL.RecipeTag>, IList<RecipeTagsDto>>(recipeDAL);
        }

        public RecipesDto AddEditRecipe(RecipesDto recipe)
        {
            DAL.Recipe recipeDAL = new DAL.Recipe();
            if (recipe.Id > 0)
            {
                recipeDAL = context.Recipes.Where(x => x.Id == recipe.Id).FirstOrDefault();
                if (recipeDAL != null)
                {
                    recipeDAL.Name = recipe.Name;
                    recipeDAL.Yield = recipe.Yield;
                    recipeDAL.ServingSize = recipe.ServingSize;
                    recipeDAL.ImageURL = recipe.ImageURL;
                    recipeDAL.IsActive = recipe.IsActive;
                    recipeDAL.Carbohydrate = recipe.Carbohydrate;
                    recipeDAL.Fat = recipe.Fat;
                    recipeDAL.Calories = recipe.Calories;
                    recipeDAL.Direction = recipe.Direction;
                    recipeDAL.Ingredients = recipe.Ingredients;
                    context.Recipes.Attach(recipeDAL);
                    context.Entry(recipeDAL).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            else
            {
                recipeDAL.Name = recipe.Name;
                recipeDAL.Yield = recipe.Yield;
                recipeDAL.ImageURL = recipe.ImageURL;
                recipeDAL.IsActive = recipe.IsActive;
                recipeDAL.Carbohydrate = recipe.Carbohydrate;
                recipeDAL.Fat = recipe.Fat;
                recipeDAL.Calories = recipe.Calories;
                recipeDAL.Direction = recipe.Direction;
                recipeDAL.Ingredients = recipe.Ingredients;
                recipeDAL.CreatedBy = recipe.CreatedBy;
                recipeDAL.DateCreated = DateTime.UtcNow;
                context.Recipes.Add(recipeDAL);
                context.SaveChanges();
            }
            recipe.Id = recipeDAL.Id;

            return recipe;
        }

        public bool AddEditRecipeTranslation(RecipeTranslationDto recipe)
        {
            var recipeDAL = context.RecipeTranslations.Where(x => x.RecipeId == recipe.RecipeId && x.LanguageCode == recipe.LanguageCode).FirstOrDefault();
            if (recipeDAL != null)
            {
                recipeDAL.Name = recipe.Name;
                recipeDAL.LanguageCode = recipe.LanguageCode;
                recipeDAL.Yield = recipe.Yield;
                recipeDAL.ServingSize = recipe.ServingSize;
                recipeDAL.Carbohydrate = recipe.Carbohydrate;
                recipeDAL.Fat = recipe.Fat;
                recipeDAL.Calories = recipe.Calories;
                recipeDAL.ImageURL = recipe.ImageURL;
                recipeDAL.IsActive = recipe.IsActive;
                recipeDAL.Direction = recipe.Direction;
                recipeDAL.Ingredients = recipe.Ingredients;
                context.RecipeTranslations.Attach(recipeDAL);
                context.Entry(recipeDAL).State = EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                recipeDAL = new DAL.RecipeTranslation();
                recipeDAL.RecipeId = recipe.RecipeId;
                recipeDAL.Name = recipe.Name;
                recipeDAL.LanguageCode = recipe.LanguageCode;
                recipeDAL.Yield = recipe.Yield;
                recipeDAL.Carbohydrate = recipe.Carbohydrate;
                recipeDAL.Fat = recipe.Fat;
                recipeDAL.Calories = recipe.Calories;
                recipeDAL.ImageURL = recipe.ImageURL;
                recipeDAL.IsActive = recipe.IsActive;
                recipeDAL.Direction = recipe.Direction;
                recipeDAL.Ingredients = recipe.Ingredients;
                recipeDAL.CreatedBy = recipe.CreatedBy;
                recipeDAL.DateCreated = DateTime.Now;
                context.RecipeTranslations.Add(recipeDAL);
                context.SaveChanges();
            }

            return true;
        }

        public bool AddEditTag(List<RecipeTagsDto> recipeTags)
        {
            if (recipeTags.Count > 0)
            {
                var recipeId = recipeTags[0].RecipeId;
                var recipes = context.RecipeTags.Where(x => x.RecipeId == recipeId).ToList();
                if (recipes != null && recipes.Count > 0)
                {
                    for (int i = 0; i < recipes.Count; i++)
                    {
                        context.RecipeTags.Remove(recipes[i]);
                    }
                    context.SaveChanges();
                }
                for (int i = 0; i < recipeTags.Count; i++)
                {
                    DAL.RecipeTag dal = new DAL.RecipeTag();
                    dal.TagId = recipeTags[i].TagId;
                    dal.IsActive = true;
                    dal.RecipeId = recipeTags[i].RecipeId;
                    context.RecipeTags.Add(dal);
                }
                context.SaveChanges();
            }
            return true;
        }

        public bool AssignRecipe(AssignedRecipeRequest request)
        {
            if (request.recipeList != null && request.recipeList.Count > 0)
            {
                var recipeId = request.recipeList[0].RecipeId;
                for (int i = 0; i < request.recipeList.Count; i++)
                {
                    DAL.AssignedRecipe dal = new DAL.AssignedRecipe();
                    dal.RecipeId = recipeId;
                    dal.OrganizationId = request.recipeList[i].OrganizationId;
                    dal.Date = DateTime.UtcNow;
                    context.AssignedRecipes.Add(dal);
                    context.SaveChanges();
                }
            }
            return true;
        }

        public IList<AssignedRecipeDto> ListAssignedRecipes()
        {
            var assignedRecipeList = context.AssignedRecipes.Include("Organization").OrderBy(x => x.Date).ToList();
            return Utility.mapper.Map<IList<DAL.AssignedRecipe>, IList<AssignedRecipeDto>>(assignedRecipeList);
        }

        public int AddRecipetoUserDashbaord()
        {
            int count = 0;
            var assignedRecipes = context.AssignedRecipes.Include("Recipe").Include("Recipe.RecipeTranslations").Where(x => x.Completed == null).ToList();
            if (assignedRecipes != null && assignedRecipes.Count > 0)
            {
                List<UserDashboardMessageDto> dashboardMessagelist = new List<UserDashboardMessageDto>();
                foreach (var recipe in assignedRecipes)
                {
                    var orgId = recipe.OrganizationId;
                    List<UserDto> userlist = new List<UserDto>();
                    var users = context.Users.Include("Organization").Include("Organization.Portals").Where(x => x.OrganizationId == orgId && x.IsActive == true && x.Organization.Portals.Any(y => y.Active == true)).ToList();
                    userlist = Utility.mapper.Map<List<DAL.User>, List<UserDto>>(users);
                    if (userlist != null && userlist.Count > 0)
                    {
                        count = count + userlist.Count;
                        for (int i = 0; i < userlist.Count(); i++)
                        {
                            var dashboardMessage = context.DashboardMessageTypes.Where(x => x.Type == IncentiveMessageTypes.AssignRecipe).FirstOrDefault();
                            if (dashboardMessage != null)
                            {
                                UserDashboardMessageDto dto = new UserDashboardMessageDto();
                                var recipeTranslations = recipe.Recipe.RecipeTranslations.Where(x => x.LanguageCode == (!string.IsNullOrEmpty(userlist[i].LanguagePreference) ? userlist[i].LanguagePreference : "en-us")).FirstOrDefault();
                                dto.Parameters = recipeTranslations != null ? recipeTranslations.Name : recipe.Recipe.Name;
                                dto.UserId = userlist[i].Id;
                                dto.Url = dashboardMessage.Url + "?" + recipe.RecipeId;
                                dto.New = true;
                                dto.MessageType = dashboardMessage.Id;
                                dto.Active = true;
                                dto.CreatedOn = DateTime.UtcNow;
                                dashboardMessagelist.Add(dto);
                            }
                        }
                    }

                }
                if (dashboardMessagelist != null && dashboardMessagelist.Count > 0)
                {
                    for (int i = 0; i < dashboardMessagelist.Count; i = i + 1000)
                    {
                        var list = dashboardMessagelist.Skip(i).Take(1000);
                        AssignRecipes(list);
                    }
                }
                foreach (var recipe in assignedRecipes)
                {
                    var recipeDal = context.AssignedRecipes.Where(x => x.Id == recipe.Id).FirstOrDefault();
                    if (recipeDal != null)
                    {
                        recipeDal.Completed = true;
                        context.AssignedRecipes.Attach(recipeDal);
                        context.Entry(recipeDal).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                }
            }
            return count;
        }

        public void AssignRecipes(IEnumerable<UserDashboardMessageDto> request)
        {
            using (var scope = new System.Transactions.TransactionScope())
            {
                /*using (var dbContext = new InterventDatabase())
                {
                    //dbContext.Configuration.AutoDetectChangesEnabled = false;
                    foreach (UserDashboardMessageDto req in request)
                    {
                        var eligibilityDbModel = new DAL.UserDashboardMessage();
                        dbContext.UserDashboardMessages.Add(CommonReader.MapToUserDashboardDAL(req, eligibilityDbModel));
                    }
                    dbContext.SaveChanges();
                }*/
                scope.Complete();
            }
        }
    }
}
