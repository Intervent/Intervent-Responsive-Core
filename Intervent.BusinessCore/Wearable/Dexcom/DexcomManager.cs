using Intervent.DAL;
using Intervent.HWS;
using Intervent.HWS.Model;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using System.Configuration;

namespace Intervent.Business
{
    public class DexcomManager : BaseManager
    {
        private static string RedirectUrl = ConfigurationManager.AppSettings["DexcomRedirectUrl"];

        public static string ApiUrl = ConfigurationManager.AppSettings["DexcomApiUrl"];

        public static string ClientId = ConfigurationManager.AppSettings["DexcomClientId"];

        public static string ClientSecret = ConfigurationManager.AppSettings["DexcomClientSecret"];

        LogReader logReader = new LogReader();

        ExternalReader externalReader = new ExternalReader();

        public static string SOURCE = "Dexcom";

        public static string INPUT_METHOD = "API";

        public string FetchDeviceDetails(string token)
        {
            try
            {
                DexcomDevices logList = Dexcom.GetDevices(ApiUrl, token);
                if (logList.Status && logList.records != null)
                {
                    return logList.userId;
                }
                else
                {
                    logReader.WriteLogMessage(new LogEventInfo(LogLevel.Debug, "DexcomManager.FetchDeviceDetails", null, "Request failed : " + logList.Status + " - " + logList.StatusCode + " - " + logList.ErrorMsg, null, logList.Exception));
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "DexcomManager.FetchDeviceDetails", null, ex.Message, null, ex));
            }
            return null;
        }

        private static List<EXT_Glucose> AddGlucose(UserDto user, DexcomGlucose request)
        {
            List<EXT_Glucose> glucoseList = new List<EXT_Glucose>();
            foreach (var data in request.records)
            {
                EXT_Glucose glucose = new EXT_Glucose();
                glucose.UserId = user.Id;
                glucose.UniqueId = !string.IsNullOrEmpty(user.UniqueId) ? user.UniqueId : "";
                glucose.EffectiveDateTime = Convert.ToDateTime(data.displayTime);
                glucose.DateTime = DateTime.UtcNow;
                glucose.OrganizationId = user.OrganizationId;
                glucose.ExtId = data.recordId;
                glucose.Source = (byte)GlucoseSource.Dexcom;
                glucose.Code = "2345-7";
                if (data.unit.Equals("mmol/L"))
                    glucose.Value = data.value * 18;
                else
                    glucose.Value = data.value;
                glucose.Unit = "mg/dL";
                glucose.IsValid = glucose.Value > 0;
                glucoseList.Add(glucose);
            }
            return glucoseList;
        }

        public void FetchDexcomLog(int deviceId, IList<UserWearableDeviceDto> devices)
        {
            try
            {
                UpdateAccesstoken(devices);

                WearableReader _wearableReader = new WearableReader();
                var activeDevices = _wearableReader.GetAllActiveUserWearableDevices(deviceId);
                if (activeDevices.Count() > 0)
                {
                    logReader.WriteLogMessage(new LogEventInfo(LogLevel.Trace, "DexcomManager.FetchDexcomLog", null, "Dexcom device count (" + activeDevices.Count() + ").", null, null));
                    foreach (var device in activeDevices)
                    {
                        FetchGlucoseLog(device);
                    }
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "DexcomManager.FetchDexcomLog", null, ex.Message, null, ex));
            }
        }

        public void FetchGlucoseLog(UserWearableDeviceDto device)
        {
            try
            {
                string startTime = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss");
                string endTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
                DexcomGlucose logList = Dexcom.GetEgvs(ApiUrl, device.Token, startTime, endTime);
                if (logList.Status)
                {
                    if (logList.records != null)
                    {
                        var list = AddGlucose(device.User, logList);
                        foreach (var data in list)
                            externalReader.AddGlucose(data);
                    }
                }
                else
                {
                    logReader.WriteLogMessage(new LogEventInfo(LogLevel.Debug, "DexcomManager.FetchGlucoseLog", null, "Request failed : " + logList.Status + " - " + logList.StatusCode + " - " + logList.ErrorMsg, null, logList.Exception));
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "DexcomManager.FetchGlucoseLog", null, ex.Message, null, ex));
            }
        }

        public void UpdateAccesstoken(IList<UserWearableDeviceDto> devices)
        {
            try
            {
                DexcomClient client = new DexcomClient(RedirectUrl);
                foreach (var device in devices)
                {
                    DexcomOAuth2 oAuth = Task.Run(() => client.RefreshTokenAsync(device.UserId, device.RefreshToken, device.ExternalUserId, device.WearableDeviceId, ApiUrl, ClientId, ClientSecret)).Result;
                    if (!oAuth.Status && oAuth.StatusCode != System.Net.HttpStatusCode.OK)
                        logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "DexcomManager.UpdateAccesstoken", null, "Refresh token failed for User : " + device.UserId + ". ExternalUserId : " + device.ExternalUserId + ", Status code : " + oAuth.StatusCode, null, null));
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "DexcomManager.UpdateAccesstoken", null, ex.Message, null, ex));
            }
        }
    }
}
