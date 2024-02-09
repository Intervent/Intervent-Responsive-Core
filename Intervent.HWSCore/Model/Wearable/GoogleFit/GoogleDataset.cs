namespace Intervent.HWS.Model
{
    public class GoogleDataSet
    {
        public class AggregateBy
        {
            public string dataTypeName { get; set; }
        }

        public List<AggregateBy> aggregateBy { get; set; }

        public long startTimeMillis { get; set; }

        public long endTimeMillis { get; set; }
    }
}
