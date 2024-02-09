using System.ComponentModel.DataAnnotations;

namespace Intervent.DAL
{
    public class EXT_Workouts
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime? StartTimeStamp { get; set; }

        public float? Duration { get; set; }

        public DateTime? EndTimeStamp { get; set; }

        public int? CaloriesBurned { get; set; }

        public float? Distance { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }

        public bool IsActive { get; set; }

        [StringLength(50)]
        public string? InputMethod { get; set; }

        [StringLength(50)]
        public string? Source { get; set; }

        [StringLength(128)]

        public string? ExternalId { get; set; }

        public virtual User user { get; set; }

    }
}
