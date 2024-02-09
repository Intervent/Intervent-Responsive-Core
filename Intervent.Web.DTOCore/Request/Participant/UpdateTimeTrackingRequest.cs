namespace Intervent.Web.DTO
{
    public class UpdateTimeTrackingRequest
    {
        public int UserId { get; set; }

        public int CoachId { get; set; }

        public bool ForceEnd { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int? Disposition { get; set; }

        public int? TimeSpent { get; set; }
    }
}
