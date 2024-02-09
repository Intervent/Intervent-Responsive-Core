namespace Intervent.HWS.Model
{
    public class WithingsMeasurements : ProcessResponse
    {
        public int status { get; set; }
        public Body body { get; set; }

        public class Body
        {
            public string updatetime { get; set; }
            public string timezone { get; set; }
            public List<Measuregrp> measuregrps { get; set; }
            public int more { get; set; }
            public int offset { get; set; }
        }

        public class Measure
        {
            public int value { get; set; }
            public int type { get; set; }
            public int unit { get; set; }
            public int algo { get; set; }
            public int fm { get; set; }
            public int fw { get; set; }
        }

        public class Measuregrp
        {
            public string grpid { get; set; }
            public int attrib { get; set; }
            public int date { get; set; }
            public int created { get; set; }
            public int modified { get; set; }
            public int category { get; set; }
            public string deviceid { get; set; }
            public List<Measure> measures { get; set; }
            public string comment { get; set; }
            public string timezone { get; set; }
        }
    }
}
