using System.ComponentModel.DataAnnotations;

namespace Intervent.DAL
{
    public class EXT_BloodPressures
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime TimeStamp { get; set; }

        public int? RestingHeartRate { get; set; }

        public int? Systolic { get; set; }

        public int? Diastolic { get; set; }

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
