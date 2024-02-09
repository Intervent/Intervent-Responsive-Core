namespace Intervent.HWS.Model
{
    public class OmronNotification : ProcessResponse
    {
        public string id { get; set; }
        public long timestamp { get; set; }
        public string type { get; set; }
    }
}
