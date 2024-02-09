using System.ComponentModel.DataAnnotations;

namespace Intervent.DAL
{
    public class EXT_Sleeps
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime StartTimeStamp { get; set; }

        public int? TotalSleepDuration { get; set; }

        public int? AwakeDuration { get; set; }

        public int? AwakeCount { get; set; }

        public int? WakeCount { get; set; }

        public int? DeepDuration { get; set; }

        public int? LightDuration { get; set; }

        public int? RemDuration { get; set; }

        public bool IsActive { get; set; }

        public int? TimetoWake { get; set; }

        public int? TimetoBed { get; set; }

        [StringLength(50)]
        public string? InputMethod { get; set; }

        [StringLength(50)]
        public string? Source { get; set; }

        [StringLength(128)]

        public string? ExternalId { get; set; }

        public int? SleepScore { get; set; }

        public virtual User user { get; set; }
    }
}
