namespace Intervent.Web.DTO
{
    public class LogReportRequest
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public int? organization { get; set; }
        public string timezone { get; set; }
        public string level { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public int? totalRecords { get; set; }
        public string textsearch { get; set; }

    }
}
