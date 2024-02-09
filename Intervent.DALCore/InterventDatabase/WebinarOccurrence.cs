using System.ComponentModel.DataAnnotations;

namespace Intervent.DAL
{
    public partial class WebinarOccurrence
    {
        public int Id { get; set; }

        public int WebinarId { get; set; }

        [Required]
        [StringLength(100)]
        public string OccurrenceId { get; set; }

        public DateTime StartTime { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        public int Duration { get; set; }

        [StringLength(200)]
        public string? VideoUrl { get; set; }

        [StringLength(200)]
        public string? Handout { get; set; }

        public virtual Webinar Webinar { get; set; }
    }
}
