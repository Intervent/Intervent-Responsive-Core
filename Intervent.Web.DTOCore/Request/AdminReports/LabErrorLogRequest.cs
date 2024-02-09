namespace Intervent.Web.DTO
{
    public class LabErrorLogRequest
    {
        public int AdminId { get; set; }

        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }

        public int? Organization { get; set; }

        public int page { get; set; }

        public int pageSize { get; set; }

        public int? totalRecords { get; set; }

        public string timezone { get; set; }

        public bool IsSuperAdmin { get; set; }
    }
}
