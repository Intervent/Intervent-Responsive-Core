namespace Intervent.HWS.Model
{
    public class WithingsSleep : ProcessResponse
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
            public int apnea_hypopnea_index { get; set; }
            public int asleepduration { get; set; }
            public int breathing_disturbances_intensity { get; set; }
            public int deepsleepduration { get; set; }
            public int durationtosleep { get; set; }
            public int durationtowakeup { get; set; }
            public int hr_average { get; set; }
            public int hr_max { get; set; }
            public int hr_min { get; set; }
            public int lightsleepduration { get; set; }
            public int nb_rem_episodes { get; set; }
            public List<object> night_events { get; set; }
            public int out_of_bed_count { get; set; }
            public int remsleepduration { get; set; }
            public int rr_average { get; set; }
            public int rr_max { get; set; }
            public int rr_min { get; set; }
            public int sleep_efficiency { get; set; }
            public int sleep_latency { get; set; }
            public int sleep_score { get; set; }
            public int snoring { get; set; }
            public int snoringepisodecount { get; set; }
            public int total_sleep_time { get; set; }
            public int total_timeinbed { get; set; }
            public int wakeup_latency { get; set; }
            public int wakeupcount { get; set; }
            public int wakeupduration { get; set; }
            public int waso { get; set; }
        }

        public class Series
        {
            public long id { get; set; }
            public string timezone { get; set; }
            public int model { get; set; }
            public int model_id { get; set; }
            public string hash_deviceid { get; set; }
            public int startdate { get; set; }
            public int enddate { get; set; }
            public string date { get; set; }
            public Data data { get; set; }
            public int created { get; set; }
            public int modified { get; set; }
        }
    }
}
