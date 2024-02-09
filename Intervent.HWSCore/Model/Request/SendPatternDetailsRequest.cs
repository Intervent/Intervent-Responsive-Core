namespace Intervent.HWS
{
    public class SendPatternDetailsRequest
    {
        public class PatternSync
        {
            public Pattern pattern { get; set; }

            public class Pattern
            {
                public string guid { get; set; }
                public string first_sync_date { get; set; }
                public string last_sync_date { get; set; }
            }
        }

        public class PatternPairing
        {
            public Pattern pattern { get; set; }

            public class Pattern
            {
                public string guid { get; set; }
                public string pattern_paired_sn { get; set; }
                public string pairing_date { get; set; }
            }
        }

        public class PatternCreation
        {
            public Pattern pattern { get; set; }

            public class Pattern
            {
                public string email { get; set; }
                public string guid { get; set; }
                public string pattern_creation_date { get; set; }
            }
        }
    }
}
