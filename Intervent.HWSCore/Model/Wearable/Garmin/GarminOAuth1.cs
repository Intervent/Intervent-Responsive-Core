using Newtonsoft.Json;

namespace Intervent.HWS.Model
{
    public class GarminOAuth1 : ProcessResponse
    {
        [JsonProperty("oauth_token")]
        public string oauth_token { get; set; }

        [JsonProperty("oauth_token_secret")]
        public string oauth_token_secret { get; set; }

        public string url { get; set; }

        public string userId { get; set; }
    }

}
