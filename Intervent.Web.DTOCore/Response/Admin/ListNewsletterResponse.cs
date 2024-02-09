namespace Intervent.Web.DTO
{
    public class ListNewsletterResponse
    {
        public IEnumerable<NewsletterDto> Newsletters { get; set; }

        public int TotalRecords { get; set; }
    }
}
