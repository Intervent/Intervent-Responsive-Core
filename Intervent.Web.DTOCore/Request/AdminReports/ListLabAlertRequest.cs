namespace Intervent.Web.DTO
{
    public class ListLabAlertRequest
    {
        public int AdminId { get; set; }
        public int? userId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string timezone { get; set; }
        public int alerttype { get; set; }
        public int labsource { get; set; }
        public int? status { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int? TotalRecords { get; set; }
        public int? Organization { get; set; }

    }
}
