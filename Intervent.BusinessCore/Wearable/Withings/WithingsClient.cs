using Intervent.HWS;
using Intervent.HWS.Model;
using Intervent.Web.DataLayer;
using NLog;
using System.Text;

namespace Intervent.Business
{
    public class WithingsClient
    {
        private string RedirectUri;

        LogReader logReader = new LogReader();

        public WithingsClient(string redirectUri)
        {
            RedirectUri = redirectUri;
        }

        public string GenerateAuthUrl(string withingsAuthUrl, string withingsClientId, string withingsMode)
        {
            var sb = new StringBuilder();
            sb.Append(withingsAuthUrl + "oauth2_user/authorize2?");
            sb.Append("response_type=code");
            sb.Append(string.Format("&client_id={0}", withingsClientId));
            sb.Append("&scope=user.activity,user.metrics,user.info");
            sb.Append(string.Format("&redirect_uri={0}", Uri.EscapeDataString(RedirectUri)));
            sb.Append("&state=login");

            if (!string.IsNullOrEmpty(withingsMode) && withingsMode.Equals("demo"))
                sb.Append("&mode=demo");

            return sb.ToString();
        }

        private async Task<string> GetNonce(string withingsApiUrl, string withingsClientId, string withingsClientSecret)
        {
            string action = "getnonce";
            int timeStamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            string message = action + "," + withingsClientId + "," + timeStamp;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("action", action),
                new KeyValuePair<string, string>("client_id", withingsClientId),
                new KeyValuePair<string, string>("timestamp", timeStamp.ToString()),
                new KeyValuePair<string, string>("signature", WearableManager.GenerateSha256Hash(withingsClientSecret, message)),
            });

            WithingsNonce response = await Withings.GetNonceAsync(content, withingsApiUrl);
            if (response.Status && response.status == 0 && response.StatusCode == System.Net.HttpStatusCode.OK && response.body != null && !string.IsNullOrEmpty(response.body.nonce))
                return response.body.nonce;
            else
                return null;
        }

        public async Task<WithingsOAuth2> GetOAuth2Async(string code, string withingsApiUrl, string withingsClientId, string withingsClientSecret)
        {
            string action = "requesttoken";
            string nonce = await GetNonce(withingsApiUrl, withingsClientId, withingsClientSecret);
            string message = action + "," + withingsClientId + "," + nonce;

            if (!string.IsNullOrEmpty(nonce))
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("action", action),
                    new KeyValuePair<string, string>("client_id", withingsClientId),
                    new KeyValuePair<string, string>("nonce", nonce),
                    new KeyValuePair<string, string>("signature", WearableManager.GenerateSha256Hash(withingsClientSecret, message)),
                    new KeyValuePair<string, string>("client_secret", withingsClientSecret),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("redirect_uri", RedirectUri)
                });

                WithingsOAuth2 response = await Withings.GetOAuth2Async(content, withingsApiUrl);
                if (response.Status && response.status == 0 && response.StatusCode == System.Net.HttpStatusCode.OK && response.body != null && !string.IsNullOrEmpty(response.body.access_token))
                    return response;
                else
                {
                    string error = !string.IsNullOrEmpty(response.error) ? response.error : "";
                    logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "WithingsClient.GetOAuth2Async", null, "Status Code : " + response.StatusCode + " ,Error : " + error, null, null));
                }
            }
            return null;
        }

        public async Task<bool> RevokeAccessAsync(int userId, string refreshToken, string externalUserId, int deviceId, string withingsApiUrl, string withingsClientId, string withingsClientSecret)
        {
            WithingsOAuth2 newAccessToken = await RefreshTokenAsync(userId, refreshToken, externalUserId, deviceId, withingsApiUrl, withingsClientId, withingsClientSecret);

            if (newAccessToken != null && newAccessToken.Status && !string.IsNullOrEmpty(newAccessToken.body.access_token))
            {
                string action = "revoke";
                string nonce = await GetNonce(withingsApiUrl, withingsClientId, withingsClientSecret);
                string message = action + "," + withingsClientId + "," + nonce;
                if (!string.IsNullOrEmpty(nonce))
                {
                    var content = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("action", action),
                    new KeyValuePair<string, string>("client_id", withingsClientId),
                    new KeyValuePair<string, string>("nonce", nonce),
                    new KeyValuePair<string, string>("signature", WearableManager.GenerateSha256Hash(withingsClientSecret, message)),
                    new KeyValuePair<string, string>("userid", externalUserId)
                });

                    WithingsRevokeUser response = await Withings.RevokeAccessAsync(content, withingsApiUrl);
                    if (response.Status && response.status == 0 && response.StatusCode == System.Net.HttpStatusCode.OK)
                        return response.Status;
                    else
                    {
                        string error = !string.IsNullOrEmpty(response.error) ? response.error : "";
                        logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "WithingsClient.RevokeAccessAsync", null, "Status Code : " + response.StatusCode + " ,Error : " + error, null, null));
                    }
                }
            }
            return false;
        }

        public async Task<WithingsOAuth2> RefreshTokenAsync(int userId, string refreshToken, string externalUserId, int deviceId, string withingsApiUrl, string withingsClientId, string withingsClientSecret)
        {
            string action = "requesttoken";
            string nonce = await GetNonce(withingsApiUrl, withingsClientId, withingsClientSecret);
            string message = action + "," + withingsClientId + "," + nonce;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("action", action),
                new KeyValuePair<string, string>("client_id", withingsClientId),
                new KeyValuePair<string, string>("nonce", nonce),
                new KeyValuePair<string, string>("signature", WearableManager.GenerateSha256Hash(withingsClientSecret, message).ToString()),
                new KeyValuePair<string, string>("client_secret", withingsClientSecret),
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", refreshToken)
            });

            WithingsOAuth2 response = await Withings.GetOAuth2Async(content, withingsApiUrl);

            WearableReader wearableReader = new WearableReader();
            AddOrEditWearableDeviceRequest deviceDetails = new AddOrEditWearableDeviceRequest();
            deviceDetails.wearableDeviceId = deviceId;
            deviceDetails.externalUserId = externalUserId;
            deviceDetails.userId = userId;

            if (response.Status && response.status == 0 && response.StatusCode == System.Net.HttpStatusCode.OK && response.body != null && !string.IsNullOrEmpty(response.body.access_token))
            {
                deviceDetails.refreshToken = response.body.refresh_token;
                deviceDetails.token = response.body.access_token;
                deviceDetails.scope = response.body.scope.Replace("user.", "");
                deviceDetails.isActive = true;
            }
            else
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "WithingsClient.RefreshTokenAsync", null, "Status Code : " + response.StatusCode, null, null));
                deviceDetails.isActive = false;
            }
            wearableReader.AddOrEditWearableDevice(deviceDetails);
            return response;
        }
    }
}
