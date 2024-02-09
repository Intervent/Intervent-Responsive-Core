namespace Intervent.Web.DTO
{
    public class DeleteCoachAvailabilityRequest
    {
        public long RefId { get; set; }

        public bool? AllFuture { get; set; }

        public DateTime startTime { get; set; }

        public DateTime endTime { get; set; }

        public DateTime? ToDate { get; set; }

        public string TimeZone { get; set; }

        public string Days { get; set; }

        public int UpdatedBy { get; set; }

    }
}
