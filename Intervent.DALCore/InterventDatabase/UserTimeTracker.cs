using System.ComponentModel.DataAnnotations.Schema;

namespace Intervent.DAL
{
    [Table("UserTimeTracker")]
    public partial class UserTimeTracker
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int CoachId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int? DispositionType { get; set; }

        public bool Billed { get; set; }

        public virtual User User { get; set; }

        public virtual TimeTrackerDisposition TimeTrackerDisposition { get; set; }
    }
}
