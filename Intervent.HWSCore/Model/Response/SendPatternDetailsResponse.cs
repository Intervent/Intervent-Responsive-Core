namespace Intervent.HWS
{
    public class SendPatternDetailsResponse : ProcessResponse
    {
        public Pattern pattern { get; set; }
        public Meta meta { get; set; }

        public class Pattern
        {
            public object id { get; set; }
            public object email { get; set; }
            public object first_sync_date { get; set; }
            public object guid { get; set; }
            public object pattern_paired_sn { get; set; }
            public object last_sync_date { get; set; }
            public object pairing_date { get; set; }
            public object pattern_creation_date { get; set; }
        }

        public class Meta
        {
            public string message { get; set; }
        }
    }
}
