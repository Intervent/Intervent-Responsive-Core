namespace Intervent.Web.DTO
{
    public class GetAppointmentsRequest
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? coachId { get; set; }

        public string TimeZone { get; set; }

        public int? userId { get; set; }

        public DateTime? portalStartDate { get; set; }

        public DateTime? portalEndDate { get; set; }

        public int page { get; set; }

        public int pageSize { get; set; }

        public int? totalRecords { get; set; }

    }
}