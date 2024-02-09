namespace Intervent.HWS
{
    public class UpdateWebinarRequest
    {
        public string agenda { get; set; }

        public int duration { get; set; }

        public string password { get; set; }

        public DateTime start_time { get; set; }

        public string timezone { get; set; }

        public string topic { get; set; }

        public int type { get; set; }
    }
}
