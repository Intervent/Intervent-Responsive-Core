namespace Intervent.Web.DTO
{
    public class NoShowApptReportRequest
    {

        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }

        public int? orgId { get; set; }

        public int? ApptType { get; set; }

        public int AdminId { get; set; }

        public string language { get; set; }

        public string timezone { get; set; }

        public string timeZoneDefault { get; set; }

        public int page { get; set; }

        public int pageSize { get; set; }

        public int? totalRecords { get; set; }

        public bool? isReviewed { get; set; }
    }
}
