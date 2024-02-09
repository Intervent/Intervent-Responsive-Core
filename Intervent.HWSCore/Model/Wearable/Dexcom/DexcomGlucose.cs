namespace Intervent.HWS.Model
{
    public class DexcomGlucose : ProcessResponse
    {
        public string recordType { get; set; }

        public string recordVersion { get; set; }

        public string userId { get; set; }

        public List<Record> records { get; set; }

        public class Record
        {
            public string recordId { get; set; }

            public DateTime systemTime { get; set; }

            public DateTime displayTime { get; set; }

            public string transmitterId { get; set; }

            public int transmitterTicks { get; set; }

            public int value { get; set; }

            public string trend { get; set; }

            public double trendRate { get; set; }

            public string unit { get; set; }

            public string rateUnit { get; set; }

            public string displayDevice { get; set; }

            public string transmitterGeneration { get; set; }
        }
    }
}
