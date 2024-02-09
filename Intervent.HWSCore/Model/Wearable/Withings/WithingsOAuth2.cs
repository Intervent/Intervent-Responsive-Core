namespace Intervent.HWS.Model
{
    public class WithingsOAuth2 : ProcessResponse
    {
        public int status { get; set; }
        public Body body { get; set; }
        public string error { get; set; }

        public class Body
        {
            public string userid { get; set; }
            public string access_token { get; set; }
            public string refresh_token { get; set; }
            public int expires_in { get; set; }
            public string scope { get; set; }
            public string csrf_token { get; set; }
            public string token_type { get; set; }
        }
    }
}
