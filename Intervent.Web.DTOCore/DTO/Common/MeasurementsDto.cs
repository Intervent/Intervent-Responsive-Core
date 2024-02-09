namespace Intervent.Web.DTO
{
    public class MeasurementsDto
    {
        public string Name { get; set; }

        public string MeasurementUnit { get; set; }

        public float? ConversionValue { get; set; }

        public float? Max { get; set; }

        public float? Min { get; set; }

        public float? Limit { get; set; }

        public string ImperialUnits { get; set; }

        public float ImperialMax { get; set; }

        public float ImperialMin { get; set; }

        public float ImperialLimit { get; set; }

        public string MetricUnits { get; set; }

        public float MetricMax { get; set; }

        public float MetricMin { get; set; }

        public float MetricLimit { get; set; }

    }
}
