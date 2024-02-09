
namespace Intervent.HWS
{
    public class PairPogoRequest
    {
        public int user_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email_address { get; set; }
        public string[] devices { get; set; }
    }
}
