namespace Intervent.Web.DTO
{
    public class GetFreeSlotsRequest
    {
        public DateTime StartDate { get; set; }

        public string StartTime { get; set; }

        public DateTime EndDate { get; set; }

        public string EndTime { get; set; }

        public int? coachId { get; set; }

        public string TimeZone { get; set; }

        public bool thirtyMinutes { get; set; }

        public bool? video { get; set; }

        public string specialities { get; set; }

        public string languages { get; set; }

        public bool IsAvailabilityByTime { get; set; }

        public string[] day { get; set; }

        public int PageSize { get; set; }

        public int StartIndex { get; set; }

        public int? TotalRecords { get; set; }

        public int OrganizationId { get; set; }

        public bool hasFilter { get; set; }

        public bool adminView { get; set; }

        public bool isLMC { get; set; }

        public int? stateId { get; set; }
    }
}