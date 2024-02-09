namespace Intervent.Web.DTO
{
    public class AdminTaskStausBulkUpdateRequest
    {
        public string taskids { get; set; }

        public string status { get; set; }

        public int userId { get; set; }
    }
}
