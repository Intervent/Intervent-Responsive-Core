using Newtonsoft.Json;

namespace Intervent.HWS.Model
{
    public class WithingsWorkouts : ProcessResponse
    {
        public int status { get; set; }
        public Body body { get; set; }

        public class Body
        {
            public List<Series> series { get; set; }
            public bool more { get; set; }
            public int offset { get; set; }
        }

        public class Data
        {
            public int algo_pause_duration { get; set; }
            public double calories { get; set; }
            public double distance { get; set; }
            public int elevation { get; set; }
            public int hr_average { get; set; }
            public int hr_max { get; set; }
            public int hr_min { get; set; }
            public int hr_zone_0 { get; set; }
            public int hr_zone_1 { get; set; }
            public int hr_zone_2 { get; set; }
            public int hr_zone_3 { get; set; }
            public int intensity { get; set; }
            public int manual_calories { get; set; }
            public int manual_distance { get; set; }
            public int pause_duration { get; set; }
            public int pool_laps { get; set; }
            public int pool_length { get; set; }
            public int spo2_average { get; set; }
            public int steps { get; set; }
            public int strokes { get; set; }
        }

        public class Series
        {
            public string id { get; set; }
            public int category { get; set; }
            public string timezone { get; set; }
            public int model { get; set; }
            public int attrib { get; set; }
            public int startdate { get; set; }
            public int enddate { get; set; }
            public string date { get; set; }
            public int modified { get; set; }
            public string deviceid { get; set; }
            [JsonProperty("data")]
            public object data { get; set; }
        }
    }
}
