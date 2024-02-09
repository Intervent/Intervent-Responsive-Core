namespace Intervent.HWS.Model
{
    public class OmronOAuth : ProcessResponse
    {
        public string id_token { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public string externalUserId { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public string errorCode { get; set; }
    }
}
