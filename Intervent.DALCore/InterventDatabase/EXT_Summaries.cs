using System.ComponentModel.DataAnnotations;

namespace Intervent.DAL
{
    public class EXT_Summaries
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime EndTimeStamp { get; set; }

        public DateTime StartTimeStamp { get; set; }

        public int? Steps { get; set; }

        public float? CaloriesBurned { get; set; }

        public float? Caloriesbmr { get; set; }

        public float? CaloriesBurnedbyActivity { get; set; }

        public float? Distance { get; set; }

        public int? ActiveDuration { get; set; }

        public float? Floors { get; set; }

        public float? Water { get; set; }

        [StringLength(50)]
        public string? InputMethod { get; set; }

        [StringLength(50)]
        public string? Source { get; set; }

        [StringLength(128)]

        public string? ExternalId { get; set; }

        public virtual User user { get; set; }
    }
}
