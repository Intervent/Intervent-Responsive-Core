namespace Intervent.Web.DTO
{
    public class GetNewsletterListRequest
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int? TotalRecords { get; set; }
    }
}
