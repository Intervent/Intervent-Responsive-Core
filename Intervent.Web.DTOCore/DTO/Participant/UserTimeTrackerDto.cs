namespace Intervent.Web.DTO
{
    public partial class UserTimeTrackerDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int CoachId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int? DispositionType { get; set; }

        public bool Billed { get; set; }

        public virtual UserDto User { get; set; }

        public virtual TimeTrackerDispositionDto TimeTrackerDisposition { get; set; }
    }
}
