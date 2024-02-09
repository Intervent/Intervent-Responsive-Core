using Intervent.HWS;
using Intervent.HWS.Model;
using Intervent.Web.DataLayer;
using NLog;
using System.Text;

namespace Intervent.Business
{
    public class DexcomClient
    {
        private string RedirectUri;

        LogReader logReader = new LogReader();

        public DexcomClient(string redirectUri)
        {
            RedirectUri = redirectUri;
        }

        public string GenerateAuthUrl(string dexcomApiUrl, string dexcomClientId)
        {
            var sb = new StringBuilder();
            sb.Append(dexcomApiUrl + "/v2/oauth2/login?");
            sb.Append("response_type=code");
            sb.Append("&scope=offline_access");
            sb.Append("&state=signin");
            sb.Append(string.Format("&client_id={0}", dexcomClientId));
            sb.Append(string.Format("&redirect_uri={0}", Uri.EscapeDataString(RedirectUri)));

            return sb.ToString();
        }

        public async Task<DexcomOAuth2> GetOAuth2Async(string code, string apiUrl, string dexcomClientId, string dexcomClientSecret)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", dexcomClientId),
                new KeyValuePair<string, string>("client_secret", dexcomClientSecret),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", RedirectUri)
            });

            DexcomOAuth2 response = await Dexcom.GetOAuth2Async(apiUrl, content);
            if (response.Status && response.StatusCode == System.Net.HttpStatusCode.OK && !string.IsNullOrEmpty(response.access_token))
                return response;
            else
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "DexcomClient.GetOAuth2Async", null, "Status Code : " + response.StatusCode, null, null));
                return null;
            }
        }

        public async Task<DexcomOAuth2> RefreshTokenAsync(int userId, string refreshToken, string externalUserId, int deviceId, string apiUrl, string dexcomClientId, string dexcomClientSecret)
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("client_id", dexcomClientId),
                new KeyValuePair<string, string>("client_secret", dexcomClientSecret),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("redirect_uri", RedirectUri)
            });

            DexcomOAuth2 response = await Dexcom.GetOAuth2Async(apiUrl, content);

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
            }
            else
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "DexcomClient.RefreshTokenAsync", null, "Status Code : " + response.StatusCode, null, null));
                deviceDetails.isActive = false;
            }
            wearableReader.AddOrEditWearableDevice(deviceDetails);
            return response;
        }
    }
}
