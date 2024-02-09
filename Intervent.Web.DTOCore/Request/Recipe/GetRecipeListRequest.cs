namespace Intervent.Web.DTO
{
    public class GetRecipeListRequest
    {
        public int? foodGroup { get; set; }

        public int? course { get; set; }

        public int? feature { get; set; }

        public string recipeName { get; set; }

        public string languageCode { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int? TotalRecords { get; set; }
    }

    public class AssignedRecipeRequest
    {
        public List<AssignedRecipeDto> recipeList { get; set; }
    }

    public class ListRecipeRequest
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public int? totalRecords { get; set; }
    }
}