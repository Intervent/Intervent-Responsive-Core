namespace Intervent.HWS.Model
{
    public class GoogleError
    {
        public Error error { get; set; }

        public class Error
        {
            public int code { get; set; }

            public string message { get; set; }

            public List<Error> errors { get; set; }

            public string status { get; set; }
        }

        public class Error2
        {
            public string message { get; set; }

            public string domain { get; set; }

            public string reason { get; set; }
        }
    }
}
