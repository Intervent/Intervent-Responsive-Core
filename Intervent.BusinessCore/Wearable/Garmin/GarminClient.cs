using Intervent.HWS;
using Intervent.HWS.Model;
using System.Text;

namespace Intervent.Business
{
    public class GarminClient
    {
        private static string RequestTokenUrl = "oauth-service/oauth/request_token";

        private static string AccessTokenUrl = "oauth-service/oauth/access_token";

        private static string GetUserUrl = "wellness-api/rest/user/id";

        private static string DeRegistrationUrl = "wellness-api/rest/user/registration";

        public async Task<GarminOAuth1> GenerateAuthUrl(string garminApiUrl, string garminConnectUrl, string garminConsumerKey, string garminConsumerSecret)
        {
            var sb = new StringBuilder();
            sb.Append(string.Format("oauth_consumer_key={0}", garminConsumerKey));
            sb.Append(string.Format("&oauth_nonce={0}", WearableManager.GenerateOauthNonce()));
            sb.Append(string.Format("&oauth_signature_method={0}", "HMAC-SHA1"));
            sb.Append(string.Format("&oauth_timestamp={0}", WearableManager.GetTimeStamp()));
            sb.Append(string.Format("&oauth_version={0}", "1.0"));
            sb.Append(string.Format("&oauth_signature={0}", WearableManager.GenerateOauthSignature("POST", garminApiUrl + RequestTokenUrl, sb.ToString(), "", garminConsumerSecret)));
            return await Garmin.GetRequestToken(RequestTokenUrl, sb, garminApiUrl, garminConnectUrl);
        }

        public async Task<GarminOAuth1> GetAccessTokenForAsync(string oauth_token, string oauth_verifier, string oauth_token_secret, string garminApiUrl, string garminUserEndPointUrl, string garminConsumerKey, string garminConsumerSecret)
        {
            GarminOAuth1 response = new GarminOAuth1();
            var sb = new StringBuilder();
            sb.Append(string.Format("oauth_consumer_key={0}", garminConsumerKey));
            sb.Append(string.Format("&oauth_nonce={0}", WearableManager.GenerateOauthNonce()));
            sb.Append(string.Format("&oauth_signature_method={0}", "HMAC-SHA1"));
            sb.Append(string.Format("&oauth_timestamp={0}", WearableManager.GetTimeStamp()));
            sb.Append(string.Format("&oauth_token={0}", oauth_token));
            sb.Append(string.Format("&oauth_verifier={0}", oauth_verifier));
            sb.Append(string.Format("&oauth_version={0}", "1.0"));
            sb.Append(string.Format("&oauth_signature={0}", WearableManager.GenerateOauthSignature("POST", garminApiUrl + AccessTokenUrl, sb.ToString(), oauth_token_secret.Split('=').LastOrDefault(), garminConsumerSecret)));

            var accessTokenDetails = await Garmin.GetAccessToken(AccessTokenUrl, sb, garminApiUrl);
            response.oauth_token = accessTokenDetails.oauth_token;
            response.oauth_token_secret = accessTokenDetails.oauth_token_secret;

            var getUser = new StringBuilder();
            getUser.Append(string.Format("oauth_consumer_key={0}", garminConsumerKey));
            getUser.Append(string.Format("&oauth_nonce={0}", WearableManager.GenerateOauthNonce()));
            getUser.Append(string.Format("&oauth_signature_method={0}", "HMAC-SHA1"));
            getUser.Append(string.Format("&oauth_timestamp={0}", WearableManager.GetTimeStamp()));
            getUser.Append(string.Format("&oauth_token={0}", response.oauth_token));
            getUser.Append(string.Format("&oauth_version={0}", "1.0"));
            getUser.Append(string.Format("&oauth_signature={0}", WearableManager.GenerateOauthSignature("GET", garminUserEndPointUrl + GetUserUrl, getUser.ToString(), response.oauth_token_secret, garminConsumerSecret)));

            var userDeatils = await Garmin.GetUserDetails(GetUserUrl, getUser, garminUserEndPointUrl);
            response.userId = userDeatils.userId;
            return response;
        }

        public async Task<GarminOAuth1> UserDeRegistration(string oauth_token, string oauth_token_secret, string garminUserEndPointUrl, string garminConsumerKey, string garminConsumerSecret)
        {
            var sb = new StringBuilder();
            sb.Append(string.Format("oauth_consumer_key={0}", garminConsumerKey));
            sb.Append(string.Format("&oauth_nonce={0}", WearableManager.GenerateOauthNonce()));
            sb.Append(string.Format("&oauth_signature_method={0}", "HMAC-SHA1"));
            sb.Append(string.Format("&oauth_timestamp={0}", WearableManager.GetTimeStamp()));
            sb.Append(string.Format("&oauth_token={0}", oauth_token));
            sb.Append(string.Format("&oauth_version={0}", "1.0"));
            sb.Append(string.Format("&oauth_signature={0}", WearableManager.GenerateOauthSignature("DELETE", garminUserEndPointUrl + DeRegistrationUrl, sb.ToString(), oauth_token_secret, garminConsumerSecret)));

            var response = await Garmin.UserDeRegistration(DeRegistrationUrl, sb, garminUserEndPointUrl);
            return response;
        }
    }
}
