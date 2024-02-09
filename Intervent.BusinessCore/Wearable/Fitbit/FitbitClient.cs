using Intervent.HWS;
using Intervent.HWS.Model;
using Intervent.Web.DataLayer;
using System.Text;

namespace Intervent.Business
{
    public class FitbitClient
    {
        private static string FitbitOAuthUrl = "https://www.fitbit.com/oauth2/authorize?";

        private string RedirectUri;

        public FitbitClient(string redirectUri)
        {
            RedirectUri = redirectUri;
        }

        public FitbitClient()
        {
        }

        public string GenerateAuthUrl(string fitbitClientId)
        {
            string[] scopeTypes = new string[] { "activity", "weight", "profile", "nutrition", "settings", "sleep", "heartrate", "cardio_fitness", "electrocardiogram", "location", "oxygen_saturation", "respiratory_rate", "social", "temperature" };
            var sb = new StringBuilder();
            sb.Append(FitbitOAuthUrl);
            sb.Append("response_type=code");
            sb.Append(string.Format("&client_id={0}", fitbitClientId));
            sb.Append(string.Format("&redirect_uri={0}", Uri.EscapeDataString(RedirectUri)));
            sb.Append(string.Format("&scope={0}", string.Join("+", scopeTypes)));

            return sb.ToString();
        }

        public async Task<FitbitOAuth2> GetOAuth2Async(string code, string fitbitApiUrl, string fitbitClientId, string fitbitClientSecret)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", fitbitClientId),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", RedirectUri)
            });

            string clientIdConcatSecret = WearableManager.Base64Encode(fitbitClientId + ":" + fitbitClientSecret);
            FitbitOAuth2 response = await Fitbit.GetOAuth2Async(content, clientIdConcatSecret, fitbitApiUrl);
            if (response.Status && response.StatusCode == System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(response.access_token))
                return response;
            else
                return null;
        }

        public async Task<FitbitOAuth2> RefreshTokenAsync(int userId, string refreshToken, string externalUserId, int deviceId, bool updateOffset, string fitbitApiUrl, string fitbitClientId, string fitbitClientSecret)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("client_id", fitbitClientId),
            });

            string clientIdConcatSecret = WearableManager.Base64Encode(fitbitClientId + ":" + fitbitClientSecret);
            FitbitOAuth2 response = await Fitbit.GetOAuth2Async(content, clientIdConcatSecret, fitbitApiUrl);

            WearableReader wearableReader = new WearableReader();
            AddOrEditWearableDeviceRequest deviceDetails = new AddOrEditWearableDeviceRequest();
            deviceDetails.wearableDeviceId = deviceId;
            deviceDetails.externalUserId = externalUserId;
            deviceDetails.userId = userId;

            if (response.Status && response.StatusCode == System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(response.access_token))
            {
                deviceDetails.refreshToken = response.refresh_token;
                deviceDetails.token = response.access_token;
                deviceDetails.isActive = true;
                if (updateOffset)
                {
                    FitbitProfile fitbitResponse = Fitbit.GetProfileDetails(response.access_token, externalUserId, fitbitApiUrl);
                    if (fitbitResponse.Status && fitbitResponse.StatusCode == System.Net.HttpStatusCode.OK && fitbitResponse.user != null)
                        deviceDetails.offsetFromUTC = fitbitResponse.user.offsetFromUTCMillis;
                }
            }
            else
            {
                deviceDetails.isActive = false;
            }
            wearableReader.AddOrEditWearableDevice(deviceDetails);
            return response;
        }

        public async Task<bool> RevokeAccessAsync(int userId, string refreshToken, string externalUserId, int deviceId, string fitbitApiUrl, string fitbitClientId, string fitbitClientSecret)
        {
            FitbitOAuth2 newAccessToken = await RefreshTokenAsync(userId, refreshToken, externalUserId, deviceId, false, fitbitApiUrl, fitbitClientId, fitbitClientSecret);

            if (newAccessToken != null && newAccessToken.Status && !string.IsNullOrEmpty(newAccessToken.access_token))
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("token", newAccessToken.access_token),
                    new KeyValuePair<string, string>("client_id", fitbitClientId),
                });

                string clientIdConcatSecret = WearableManager.Base64Encode(fitbitClientId + ":" + fitbitClientSecret);
                ProcessResponse response = await Fitbit.RevokeAccessAsync(content, clientIdConcatSecret, fitbitApiUrl);
                if (response.Status && response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
            }
            return false;
        }

        public FitbitProfile GetProfileDetails(string accessToken, string externalUserId, string fitbitApiUrl)
        {
            FitbitProfile response = Fitbit.GetProfileDetails(accessToken, externalUserId, fitbitApiUrl);
            if (response.Status && response.StatusCode == System.Net.HttpStatusCode.OK && response.user != null)
                return response;
            else
                return null;
        }

        public FitbitScope GetScopeDetails(string accessToken, string fitbitApiUrl)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("token", accessToken)
            });
            FitbitScope response = Fitbit.GetUsersScope(content, accessToken, fitbitApiUrl);
            if (response != null && response.Status && response.StatusCode == System.Net.HttpStatusCode.OK)
                return response;
            else
                return null;
        }
    }
}
