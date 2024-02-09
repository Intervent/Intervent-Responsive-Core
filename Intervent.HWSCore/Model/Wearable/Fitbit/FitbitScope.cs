namespace Intervent.HWS.Model
{
    public class FitbitScope : ProcessResponse
    {
        public bool active { get; set; }
        public string scope { get; set; }
        public string client_id { get; set; }
        public string user_id { get; set; }
        public string token_type { get; set; }
        public long exp { get; set; }
        public long iat { get; set; }
    }
}
