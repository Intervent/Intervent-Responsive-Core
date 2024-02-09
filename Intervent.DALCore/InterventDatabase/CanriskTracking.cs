using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intervent.DAL
{
    public class CanriskTracking
    {
        [Key]
        public string Guid { get; set; }

        public int pageCompleted { get; set; }

        [StringLength(512)]
        public string? utm_source { get; set; }

        [StringLength(512)]
        public string? utm_medium { get; set; }

        [StringLength(512)]
        public string? utm_campaign { get; set; }

        [StringLength(512)]
        public string? utm_keywords { get; set; }

        [StringLength(512)]
        public string? Reason { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DOB { get; set; }

        public byte? Gender { get; set; }

        public DateTime CreatedOn { get; set; }

        public int? ReasonId { get; set; }

        public virtual Eligibility Eligibility { get; set; }

        public int? EligibilityId { get; set; }
    }
}
