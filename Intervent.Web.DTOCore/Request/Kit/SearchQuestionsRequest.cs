namespace Intervent.Web.DTO
{
    public class SearchQuestionsRequest
    {
        public int kitId { get; set; }

        public string searchText { get; set; }

        public int? questionType { get; set; }

        public int? activityId { get; set; }

    }
}
