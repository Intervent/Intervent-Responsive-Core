using Intervent.HWS;
using Intervent.HWS.Model;
using Intervent.Web.DataLayer;
using System.Text;

namespace Intervent.Business
{
    public class GoogleFitClient
    {
        private string RedirectUri;

        public GoogleFitClient(string redirectUri)
        {
            RedirectUri = redirectUri;
        }

        public string GenerateAuthUrl(string googleFitClientId)
        {
            var sb = new StringBuilder();
            sb.Append("https://accounts.google.com/o/oauth2/v2/auth?");
            sb.Append(string.Format("client_id={0}", googleFitClientId));
            sb.Append(string.Format("&redirect_uri={0}", Uri.EscapeDataString(RedirectUri)));
            sb.Append("&response_type=code");
            sb.Append("&access_type=offline");
            sb.Append(string.Format("&scope={0}", Uri.EscapeDataString("https://www.googleapis.com/auth/fitness.activity.read https://www.googleapis.com/auth/fitness.blood_glucose.read https://www.googleapis.com/auth/fitness.blood_pressure.read https://www.googleapis.com/auth/fitness.body.read https://www.googleapis.com/auth/fitness.body_temperature.read https://www.googleapis.com/auth/fitness.heart_rate.read https://www.googleapis.com/auth/fitness.location.read https://www.googleapis.com/auth/fitness.nutrition.read https://www.googleapis.com/auth/fitness.oxygen_saturation.read https://www.googleapis.com/auth/fitness.reproductive_health.read https://www.googleapis.com/auth/fitness.sleep.read https://www.googleapis.com/auth/userinfo.profile")));
            sb.Append("&state=code");
            sb.Append("&prompt=select_account");
            return sb.ToString();
        }

        public async Task<GoogleFitOAuth> GetOAuth2Async(string code, string googleFitClientId, string googleFitClientSecret)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", googleFitClientId),
                new KeyValuePair<string, string>("client_secret", googleFitClientSecret),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", RedirectUri)
            });

            GoogleFitOAuth response = await GoogleFit.GetOAuthAsync(content);
            if (response.Status && response.StatusCode == System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(response.access_token))
                return response;
            else
                return null;
        }

        public async Task<GoogleFitOAuth> RefreshTokenAsync(int userId, string refreshToken, string externalUserId, int deviceId, bool updateOffset, string googleFitClientId, string googleFitClientSecret)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("client_id", googleFitClientId),
                new KeyValuePair<string, string>("client_secret", googleFitClientSecret),
            });

            GoogleFitOAuth response = await GoogleFit.GetOAuthAsync(content);

            WearableReader wearableReader = new WearableReader();
            AddOrEditWearableDeviceRequest deviceDetails = new AddOrEditWearableDeviceRequest();
            deviceDetails.wearableDeviceId = deviceId;
            deviceDetails.externalUserId = externalUserId;
            deviceDetails.userId = userId;

            if (response.Status && response.StatusCode == System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(response.access_token))
            {
                deviceDetails.refreshToken = refreshToken;
                deviceDetails.token = response.access_token;
                deviceDetails.isActive = true;
            }
            else
            {
                deviceDetails.isActive = false;
            }
            wearableReader.AddOrEditWearableDevice(deviceDetails);
            return response;
        }

        public GoogleFitProfile GetProfileDetails(string accessToken)
        {
            GoogleFitProfile response = GoogleFit.GetProfileDetails(accessToken);
            if (response.Status && response.StatusCode == System.Net.HttpStatusCode.OK && response.id != null)
                return response;
            else
                return null;
        }

        public async Task<bool> RevokeAccessAsync(int userId, string refreshToken, string externalUserId, int deviceId, string googleFitClientId, string googleFitClientSecret)
        {
            GoogleFitOAuth newAccessToken = await RefreshTokenAsync(userId, refreshToken, externalUserId, deviceId, false, googleFitClientId, googleFitClientSecret);

            if (newAccessToken != null && newAccessToken.Status && !string.IsNullOrEmpty(newAccessToken.access_token))
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("token", newAccessToken.access_token),
                });

                ProcessResponse response = await GoogleFit.RevokeAccessAsync(content);
                if (response.Status && response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
            }
            return false;
        }
    }
}
