namespace Intervent.Web.DTO
{
    public class GetRecipeListResponse
    {
        public IList<RecipesDto> recipes { get; set; }

        public int totalRecords { get; set; }
    }
}
