namespace Intervent.HWS
{
    public class RegisterUserForWebinarResponse : ProcessResponse
    {
        public string registrant_id { get; set; }

        public string id { get; set; }

        public string topic { get; set; }

        public DateTime start_time { get; set; }

        public string join_url { get; set; }
    }
}
