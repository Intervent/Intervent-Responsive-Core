namespace Intervent.Web.DTO
{
    public class GetTaskListRequest
    {
        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }

        public int[] taskTypeId { get; set; }

        public int? ownerId { get; set; }

        public string status { get; set; }

        public int page { get; set; }

        public int pageSize { get; set; }

        public int? totalRecords { get; set; }

        public string timezone { get; set; }

        public int userId { get; set; }

        public string Organizations { get; set; }
    }
}
