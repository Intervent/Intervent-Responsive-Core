namespace Intervent.Web.DTO
{
    public class GetWebinarListResponse
    {
        public IList<WebinarDto> webinars { get; set; }

        public int totalRecords { get; set; }
    }
}
