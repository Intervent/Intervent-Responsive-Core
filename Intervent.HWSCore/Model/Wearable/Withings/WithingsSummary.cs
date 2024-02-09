namespace Intervent.HWS.Model
{
    public class WithingsSummary : ProcessResponse
    {
        public int status { get; set; }
        public Body body { get; set; }

        public class Activity
        {
            public int steps { get; set; }
            public double distance { get; set; }
            public int elevation { get; set; }
            public int soft { get; set; }
            public int moderate { get; set; }
            public int intense { get; set; }
            public int active { get; set; }
            public double calories { get; set; }
            public double totalcalories { get; set; }
            public object deviceid { get; set; }
            public object hash_deviceid { get; set; }
            public string timezone { get; set; }
            public string date { get; set; }
            public int modified { get; set; }
            public int brand { get; set; }
            public bool is_tracker { get; set; }
        }

        public class Body
        {
            public List<Activity> activities { get; set; }
            public bool more { get; set; }
            public int offset { get; set; }
        }

    }
}
