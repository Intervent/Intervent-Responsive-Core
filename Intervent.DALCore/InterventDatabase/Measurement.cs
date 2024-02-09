namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;

    public partial class Measurement
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string ImperialUnits { get; set; }

        [StringLength(50)]
        public string? MetricUnits { get; set; }

        public float? ConversionValue { get; set; }

        public float? ImperialMax { get; set; }

        public float? ImperialMin { get; set; }

        public float? ImperialLimit { get; set; }

        public float? MetricMax { get; set; }

        public float? MetricMin { get; set; }

        public float? MetricLimit { get; set; }
    }
}
