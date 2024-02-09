using Intervent.HWS;
using Intervent.HWS.Model;
using Intervent.Web.DataLayer;
using NLog;
using System.Text;

namespace Intervent.Business
{
    public class OmronClient
    {
        private string RedirectUri;

        LogReader logReader = new LogReader();

        public OmronClient(string redirectUri)
        {
            RedirectUri = redirectUri;
        }

        public string GenerateAuthUrl(string omronAuthUrl, string omronClientId)
        {
            var sb = new StringBuilder();
            sb.Append(omronAuthUrl + "connect/authorize?");
            sb.Append(string.Format("client_id={0}", omronClientId));
            sb.Append("&response_type=code");
            sb.Append("&scope=bloodpressure+activity+weight+temperature+oxygen+openid+offline_access");
            sb.Append(string.Format("&redirect_uri={0}", Uri.EscapeDataString(RedirectUri)));
            sb.Append("&state=signin");

            return sb.ToString();
        }

        public async Task<OmronOAuth> GetOAuth2Async(string code, string omronApiUrl, string omronClientId, string omronClientSecret)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", omronClientId),
                new KeyValuePair<string, string>("client_secret", omronClientSecret),
                new KeyValuePair<string, string>("scope", "bloodpressure+activity+weight+temperature+oxygen+openid+offline_access"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", RedirectUri)
            });

            OmronOAuth response = await Omron.GetOAuthAsync(content, omronApiUrl);
            if (response.Status && response.StatusCode == System.Net.HttpStatusCode.OK && response.success && !string.IsNullOrEmpty(response.access_token))
                return response;
            else
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "OmronClient.GetOAuth2Async", null, "Status Code : " + response.StatusCode, null, null));
                return null;
            }
        }

        public async Task<OmronOAuth> RefreshTokenAsync(int userId, string refreshToken, string externalUserId, int deviceId, string omronApiUrl, string omronClientId, string omronClientSecret)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("client_id", omronClientId),
                new KeyValuePair<string, string>("client_secret", omronClientSecret),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("redirect_uri", RedirectUri)
            });

            OmronOAuth response = await Omron.GetOAuthAsync(content, omronApiUrl);

            WearableReader wearableReader = new WearableReader();
            AddOrEditWearableDeviceRequest deviceDetails = new AddOrEditWearableDeviceRequest();
            deviceDetails.wearableDeviceId = deviceId;
            deviceDetails.externalUserId = externalUserId;
            deviceDetails.userId = userId;

            if (response.Status && response.StatusCode == System.Net.HttpStatusCode.OK && response.success && !string.IsNullOrEmpty(response.access_token))
            {
                deviceDetails.refreshToken = response.refresh_token;
                deviceDetails.token = response.access_token;
                deviceDetails.isActive = true;
            }
            else
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "OmronClient.RefreshTokenAsync", null, "Status Code : " + response.StatusCode, null, null));
                deviceDetails.isActive = false;
            }
            wearableReader.AddOrEditWearableDevice(deviceDetails);
            return response;
        }

        public async Task<bool> RevokeAccessAsync(int userId, string refreshToken, string externalUserId, int deviceId, string omronApiUrl, string omronClientId, string omronClientSecret)
        {
            OmronOAuth newAccessToken = await RefreshTokenAsync(userId, refreshToken, externalUserId, deviceId, omronApiUrl, omronClientId, omronClientSecret);

            if (newAccessToken != null && newAccessToken.Status && !string.IsNullOrEmpty(newAccessToken.access_token))
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("token_type_hint", "access_token"),
                    new KeyValuePair<string, string>("token", newAccessToken.access_token),
                    new KeyValuePair<string, string>("client_secret", omronClientSecret),
                    new KeyValuePair<string, string>("client_id", omronClientId),
                });

                ProcessResponse response = await Omron.RevokeAccessAsync(content, omronApiUrl);
                if (response.Status && response.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
            }
            return false;
        }
    }
}
