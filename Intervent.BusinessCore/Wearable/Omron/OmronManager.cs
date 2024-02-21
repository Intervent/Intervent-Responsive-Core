using Intervent.DAL;
using Intervent.HWS;
using Intervent.HWS.Model;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using System.Configuration;

namespace Intervent.Business
{
    public class OmronManager : BaseManager
    {
        LogReader logReader = new LogReader();

        ExternalReader externalReader = new ExternalReader();

        public static string SOURCE = "Omron";

        public static string INPUT_METHOD = "API";

        public static string ApiUrl = ConfigurationManager.AppSettings["OmronApiUrl"];

        public static string RedirectUrl = ConfigurationManager.AppSettings["OmronRedirectUrl"];

        public static string ClientId = ConfigurationManager.AppSettings["OmronClientId"];

        public static string ClientSecret = ConfigurationManager.AppSettings["OmronClientSecret"];

		public int SystemAdminId = Convert.ToInt32(ConfigurationManager.AppSettings["SystemAdminId"]);

		public OmronManager() { }

        public OmronManager(int systemAdminId, string omronApiUrl, string omronClientId, string omronClientSecret, string omronRedirectUrl)
        {
            SystemAdminId = systemAdminId;
            ApiUrl = omronApiUrl;
            ClientId = omronClientId;
            ClientSecret = omronClientSecret;
            RedirectUrl = omronRedirectUrl;
        }

        private static EXT_Weights AddWeight(int userId, OmronMeasurement.Weight request)
        {
            EXT_Weights weight = new EXT_Weights();
            weight.UserId = userId;
            weight.IsActive = true;
            weight.TimeStamp = request.dateTime;
            weight.Source = SOURCE;
            weight.ExternalId = request.id.ToString();
            weight.InputMethod = INPUT_METHOD;
            if (!string.IsNullOrEmpty(request.weight))
                weight.Weight = Convert.ToDouble(request.weight);
            if (!string.IsNullOrEmpty(request.bmiValue))
                weight.bmi = Convert.ToDouble(request.bmiValue);
            if (!string.IsNullOrEmpty(request.bodyFatPercentage))
                weight.FatPercent = Convert.ToDouble(request.bodyFatPercentage);
            return weight;
        }

        private static EXT_BloodPressures AddBloodPressure(int userId, OmronMeasurement.BloodPressure request)
        {
            EXT_BloodPressures bloodPressure = new EXT_BloodPressures();
            bloodPressure.UserId = userId;
            bloodPressure.ExternalId = request.id.ToString();
            bloodPressure.Source = SOURCE;
            bloodPressure.TimeStamp = request.dateTime;
            bloodPressure.InputMethod = INPUT_METHOD;
            if (!string.IsNullOrEmpty(request.pulse))
                bloodPressure.RestingHeartRate = int.Parse(request.pulse);
            if (!string.IsNullOrEmpty(request.systolic))
                bloodPressure.Systolic = int.Parse(request.systolic);
            if (!string.IsNullOrEmpty(request.diastolic))
                bloodPressure.Diastolic = int.Parse(request.diastolic);
            bloodPressure.IsActive = true;
            return bloodPressure;
        }

        private static List<EXT_Workouts> AddFitness(int userId, OmronMeasurement.Activity request)
        {
            List<EXT_Workouts> workoutList = new List<EXT_Workouts>();
            foreach (var hourlyActivityData in request.hourlyActivityData)
            {
                EXT_Workouts workout = new EXT_Workouts();
                workout.UserId = userId;
                workout.Source = SOURCE;
                workout.StartTimeStamp = request.dateTime;
                if (!string.IsNullOrEmpty(hourlyActivityData.hour))
                {
                    workout.Duration = float.Parse(hourlyActivityData.hour) / 3600;
                    workout.EndTimeStamp = workout.StartTimeStamp.Value.AddSeconds(workout.Duration.Value);
                }
                workout.ExternalId = request.id.ToString();
                workout.Name = "";
                workout.Category = "";
                workout.InputMethod = INPUT_METHOD;
                workout.IsActive = true;
                if (hourlyActivityData.calories != 0)
                    workout.CaloriesBurned = hourlyActivityData.calories;
                if (hourlyActivityData.distance != 0)
                    workout.Distance = hourlyActivityData.distance;
                workoutList.Add(workout);
            }
            return workoutList;
        }

        private static EXT_Summaries AddSummaries(int userId, OmronMeasurement.Activity request)
        {
            EXT_Summaries summary = new EXT_Summaries();
            summary.UserId = userId;
            summary.Source = SOURCE;
            summary.StartTimeStamp = request.dateTime;
            //summary.EndTimeStamp = date.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddHours(UTC_OFFSET);
            summary.ExternalId = request.id.ToString();
            summary.InputMethod = INPUT_METHOD;
            if (!string.IsNullOrEmpty(request.dailyDistance))
                summary.Distance = float.Parse(request.dailyDistance);
            if (request.calories != 0)
                summary.CaloriesBurned = request.calories;
            if (request.steps != 0)
                summary.Steps = request.steps;
            //summary.Floors = request.summary.floors;
            //summary.ActiveDuration = request.activities.Select(x => x.duration).Sum() / 60;
            //summary.CaloriesBurnedbyActivity = request.summary.activityCalories;
            //summary.Caloriesbmr = request.summary.caloriesBMR;
            if (summary.Distance > 0 || summary.ActiveDuration > 0 || summary.CaloriesBurned > 0 || summary.Floors > 0 || summary.Steps > 0 || summary.CaloriesBurnedbyActivity > 0 || summary.Caloriesbmr > 0 || summary.Water > 0)
                return summary;
            else
                return null;
        }

        private void UpdateAccesstoken(IList<UserWearableDeviceDto> devices)
        {
            try
            {
                OmronClient client = new OmronClient(RedirectUrl);
                foreach (var device in devices)
                {
                    UpdateAccesstoken(device, client);
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "OmronManager.UpdateAccesstoken", null, ex.Message, null, ex));
            }
        }

        public void FetchOmronLog(int deviceId, IList<UserWearableDeviceDto> usersWithingsDevices)
        {
            UpdateAccesstoken(usersWithingsDevices);
            try
            {
                WearableReader wearableReader = new WearableReader();
                var activeUsersOmronDevices = wearableReader.GetAllActiveUserWearableDevices(deviceId);
                if (activeUsersOmronDevices.Count > 0)
                {
                    logReader.WriteLogMessage(new LogEventInfo(LogLevel.Trace, "OmronManager.FetchOmronLog", null, "Omron device count (" + activeUsersOmronDevices.Count() + ").", null, null));
                    foreach (var device in activeUsersOmronDevices)
                    {
                        FetchMeasurementLog(device);
                    }
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "OmronManager.FetchOmronLog", null, ex.Message, null, ex));
            }
        }

        public void FetchMeasurementLog(UserWearableDeviceDto device, string type = null)
        {
            try
            {
                string token = device.Token;

                if (string.IsNullOrEmpty(type))
                {
                    OmronClient client = new OmronClient(RedirectUrl);
                    token = UpdateAccesstoken(device, client);
                }
                if (!string.IsNullOrEmpty(token))
                {
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("type", type),
                        new KeyValuePair<string, string>("since", DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-dd"))
                    });

                    OmronMeasurement logList = Task.Run(() => Omron.GetMeasurement(content, token, ApiUrl)).Result;
                    if (logList.Status)
                    {
                        if (logList.result != null && logList.result.weight != null && logList.result.weight.Count() > 0)
                        {
                            int offsetFromUTC = device.OffsetFromUTC.HasValue ? device.OffsetFromUTC.Value : 0;
                            foreach (var weight in logList.result.weight)
                                externalReader.AddExtWeight(AddWeight(device.UserId, weight), Convert.ToInt32(ConfigurationManager.AppSettings["SystemAdminId"]));
                        }

                        if (logList.result != null && logList.result.bloodPressure != null && logList.result.bloodPressure.Count() > 0)
                        {
                            foreach (var bloodPressure in logList.result.bloodPressure)
                                externalReader.AddExtBloodPressure(AddBloodPressure(device.UserId, bloodPressure));
                        }

                        if (logList.result != null && logList.result.activity != null && logList.result.activity.Count() > 0)
                        {
                            foreach (var activity in logList.result.activity)
                            {
                                var summary = AddSummaries(device.UserId, activity);
                                if (summary != null)
                                    externalReader.AddExtSummary(summary);

                                if (activity.hourlyActivityData.Count() > 0)
                                {
                                    List<EXT_Workouts> workouts = AddFitness(device.UserId, activity);
                                    foreach (var workout in workouts)
                                        externalReader.AddExtWorkout(workout);
                                }
                            }
                        }
                    }
                    else
                    {
                        logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "OmronManager.FetchMeasurementLog", null, "Request failed : " + logList.Status + " - " + logList.StatusCode + " - " + logList.ErrorMsg, null, logList.Exception));
                    }
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "OmronManager.FetchMeasurementLog", null, ex.Message, null, ex));
            }
        }

        public string UpdateAccesstoken(UserWearableDeviceDto device, OmronClient client)
        {
            try
            {
                OmronOAuth oAuth = Task.Run(() => client.RefreshTokenAsync(device.UserId, device.RefreshToken, device.ExternalUserId, device.WearableDeviceId, ApiUrl, ClientId, ClientSecret)).Result;
                if (!oAuth.Status && oAuth.StatusCode != System.Net.HttpStatusCode.OK)
                    logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "OmronManager.UpdateAccesstoken", null, "Refresh token failed for User : " + device.UserId + ". ExternalUserId : " + device.ExternalUserId + ", Status code : " + oAuth.StatusCode, null, null));
                else
                    return oAuth.access_token;
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "OmronManager.UpdateAccesstoken", null, ex.Message, null, ex));
            }
            return null;
        }
    }
}
