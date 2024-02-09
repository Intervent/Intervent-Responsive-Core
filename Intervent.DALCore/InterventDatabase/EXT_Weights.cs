using System.ComponentModel.DataAnnotations;

namespace Intervent.DAL
{
    public class EXT_Weights
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime TimeStamp { get; set; }

        public double? Weight { get; set; }

        public double? bmi { get; set; }

        public double? FatPercent { get; set; }

        public bool IsActive { get; set; }

        [StringLength(50)]
        public string? InputMethod { get; set; }

        public string? Source { get; set; }

        [StringLength(128)]

        public string? ExternalId { get; set; }

        public virtual User user { get; set; }
    }
}
