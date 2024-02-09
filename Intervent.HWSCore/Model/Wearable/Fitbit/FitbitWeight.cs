namespace Intervent.HWS.Model
{
    public class FitbitWeight : ProcessResponse
    {
        public List<Weight> weight { get; set; }
    }

    public class Weight
    {
        public double bmi { get; set; }

        public string date { get; set; }

        public object logId { get; set; }

        public string source { get; set; }

        public string time { get; set; }

        public double weight { get; set; }

        public double? fat { get; set; }
    }
}
