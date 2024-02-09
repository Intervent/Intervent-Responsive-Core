using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Intervent.Business
{
    public class WearableManager
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string GenerateSha256Hash(string key, string message)
        {
            string hash;
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] code = encoder.GetBytes(key);
            using (HMACSHA256 hmac = new HMACSHA256(code))
            {
                byte[] hmBytes = hmac.ComputeHash(encoder.GetBytes(message));
                hash = ToHexString(hmBytes);
            }
            return hash;
        }

        public static string ToHexString(byte[] array)
        {
            StringBuilder hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        public static string GenerateOauthNonce()
        {
            int length = 7;
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();
            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            return str_build.ToString();
        }

        public static int GetTimeStamp()
        {
            DateTime Expiry = DateTime.UtcNow;
            return (int)(Expiry - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static string GenerateOauthSignature(string methodType, string baseUrl, string content, string oauth_token, string consumerSecret)
        {
            Regex reg = new Regex(@"%[a-f0-9]{2}");
            string encodedRequest = methodType + "&" + reg.Replace(WebUtility.UrlEncode(baseUrl), m => m.Value.ToUpperInvariant()) + "&" + reg.Replace(WebUtility.UrlEncode(content), m => m.Value.ToUpperInvariant());
            string secretKey = consumerSecret + "&" + oauth_token;
            return reg.Replace(WebUtility.UrlEncode(GenerateSha1Hash(encodedRequest, secretKey)), m => m.Value.ToUpperInvariant());
        }

        private static string GenerateSha1Hash(string text, string key)
        {
            using (var hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(key)))
            {
                var hash = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(text));
                return Convert.ToBase64String(hash);
            }
        }

        public void FetchWearableLogs()
        {
            LogReader logReader = new LogReader();
            try
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Trace, "WearableManager.FetchWearableLogs", null, "Fetch Wearable log initiated : " + DateTime.UtcNow.ToString(), null, null));
                WearableReader _wearableReader = new WearableReader();
                var activeDevices = _wearableReader.GetWearableDevices(1).ToList();

                foreach (var device in activeDevices)
                {
                    IList<UserWearableDeviceDto> usersActiveDevices = _wearableReader.GetAllActiveUserWearableDevices(device.Id);
                    if (usersActiveDevices.Count() > 0)
                    {
                        switch (device.Name.ToLower())
                        {
                            case "fitbit":
                                new FitbitManager().FetchFitBitLog(device.Id, usersActiveDevices);
                                break;
                            case "dexcom":
                                new DexcomManager().FetchDexcomLog(device.Id, usersActiveDevices);
                                break;
                            case "withings":
                                new WithingsManager().FetchWithingsLog(device.Id, usersActiveDevices);
                                break;
                            case "omron":
                                new OmronManager().FetchOmronLog(device.Id, usersActiveDevices);
                                break;
                        }
                    }
                }
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Trace, "WearableManager.FetchWearableLogs", null, "Fetch Wearable log ended : " + DateTime.UtcNow.ToString(), null, null));
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "WearableManager.FetchWearableLogs", null, ex.Message, null, ex));
            }

        }
    }
}
