namespace Intervent.HWS.Model
{
    public class GoogleFitness : ProcessResponse
    {
        public List<Bucket> bucket { get; set; }

        public GoogleError error { get; set; }

        public class Bucket
        {
            public string startTimeMillis { get; set; }
            public string endTimeMillis { get; set; }
            public List<Dataset> dataset { get; set; }
        }

        public class Dataset
        {
            public string dataSourceId { get; set; }
            public List<Point> point { get; set; }
        }

        public class Point
        {
            public string startTimeNanos { get; set; }
            public string endTimeNanos { get; set; }
            public string dataTypeName { get; set; }
            public string originDataSourceId { get; set; }
            public List<Value> value { get; set; }
        }

        public class Value
        {
            public List<MapVal> mapVal { get; set; }
            public int? intVal { get; set; }
            public string stringVal { get; set; }
            public double? fpVal { get; set; }
        }

        public class MapVal
        {
            public string key { get; set; }
            public Value value { get; set; }
        }
    }
}
