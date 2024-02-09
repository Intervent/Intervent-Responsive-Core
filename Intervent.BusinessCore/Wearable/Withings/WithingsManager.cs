using Intervent.DAL;
using Intervent.HWS;
using Intervent.HWS.Model;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Newtonsoft.Json;
using NLog;
using System.Configuration;
using TimeZoneConverter;

namespace Intervent.Business
{
    public class WithingsManager : BaseManager
    {
        ExternalReader externalReader = new ExternalReader();

        LogReader logReader = new LogReader();

        private static string SOURCE = "Withings";

        private static string INPUT_METHOD = "API";

        private static DateTime unixTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static string ApiUrl = ConfigurationManager.AppSettings["WithingsApiUrl"];

        public static string ClientId = ConfigurationManager.AppSettings["WithingsClientId"];

        public static string ClientSecret = ConfigurationManager.AppSettings["WithingsClientSecret"];

        private static string GetWorkoutType(int workoutType)
        {
            if (workoutType == 1)
                return "Walk";
            else if (workoutType == 2)
                return "Run";
            else if (workoutType == 3)
                return "Hiking";
            else if (workoutType == 4)
                return "Skating";
            else if (workoutType == 5)
                return "BMX";
            else if (workoutType == 6)
                return "Bicycling";
            else if (workoutType == 7)
                return "Swimming";
            else if (workoutType == 8)
                return "Surfing";
            else if (workoutType == 9)
                return "Kitesurfing";
            else if (workoutType == 10)
                return "Windsurfing";
            else if (workoutType == 11)
                return "Bodyboard";
            else if (workoutType == 12)
                return "Tennis";
            else if (workoutType == 13)
                return "Table tennis";
            else if (workoutType == 14)
                return "Squash";
            else if (workoutType == 15)
                return "Badminton";
            else if (workoutType == 16)
                return "Lift weights";
            else if (workoutType == 17)
                return "Calisthenics";
            else if (workoutType == 18)
                return "Elliptical";
            else if (workoutType == 19)
                return "Pilates";
            else if (workoutType == 20)
                return "Basket-ball";
            else if (workoutType == 21)
                return "Soccer";
            else if (workoutType == 22)
                return "Football";
            else if (workoutType == 23)
                return "Rugby";
            else if (workoutType == 24)
                return "Volley-ball";
            else if (workoutType == 25)
                return "Waterpolo";
            else if (workoutType == 26)
                return "Horse riding";
            else if (workoutType == 27)
                return "Golf";
            else if (workoutType == 28)
                return "Yoga";
            else if (workoutType == 29)
                return "Dancing";
            else if (workoutType == 30)
                return "Boxing";
            else if (workoutType == 31)
                return "Fencing";
            else if (workoutType == 32)
                return "Wrestling";
            else if (workoutType == 33)
                return "Martial arts";
            else if (workoutType == 34)
                return "Skiing";
            else if (workoutType == 35)
                return "Snowboarding";
            else if (workoutType == 36)
                return "Other";
            else if (workoutType == 128)
                return "No activity";
            else if (workoutType == 187)
                return "Rowing";
            else if (workoutType == 188)
                return "Zumba";
            else if (workoutType == 191)
                return "Baseball";
            else if (workoutType == 192)
                return "Handball";
            else if (workoutType == 193)
                return "Hockey";
            else if (workoutType == 194)
                return "Ice hockey";
            else if (workoutType == 195)
                return "Climbing";
            else if (workoutType == 196)
                return "Ice skating";
            else if (workoutType == 272)
                return "Multi-sport";
            else if (workoutType == 306)
                return "Indoor walk";
            else if (workoutType == 307)
                return "Indoor running";
            else if (workoutType == 308)
                return "Indoor cycling";
            else
                return "N/A";
        }

        private enum MeasurementType
        {
            Weight = 1,
            FatRatio = 6,
            DBP = 9,
            SBP = 10
        }

        private void UpdateAccesstoken(IList<UserWearableDeviceDto> devices)
        {
            try
            {
                WithingsClient client = new WithingsClient(null);
                foreach (var device in devices)
                {
                    WithingsOAuth2 oAuth = Task.Run(() => client.RefreshTokenAsync(device.UserId, device.RefreshToken, device.ExternalUserId, device.WearableDeviceId, ApiUrl, ClientId, ClientSecret)).Result;
                    if (!oAuth.Status && oAuth.StatusCode != System.Net.HttpStatusCode.OK)
                        logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "WithingsManager.UpdateAccesstoken", null, "Refresh token failed for User : " + device.UserId + ". ExternalUserId : " + device.ExternalUserId + ", Status code : " + oAuth.StatusCode, null, null));
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "WithingsManager.UpdateAccesstoken", null, ex.Message, null, ex));
            }
        }

        public void FetchWithingsLog(int deviceId, IList<UserWearableDeviceDto> usersWithingsDevices)
        {
            try
            {
                UpdateAccesstoken(usersWithingsDevices);

                WearableReader wearableReader = new WearableReader();
                var activeUsersWithingsDevices = wearableReader.GetAllActiveUserWearableDevices(deviceId);
                if (activeUsersWithingsDevices.Count > 0)
                {
                    logReader.WriteLogMessage(new LogEventInfo(LogLevel.Trace, "WithingsManager.FetchWithingsLog", null, "Withings device count (" + activeUsersWithingsDevices.Count() + ").", null, null));
                    foreach (var device in activeUsersWithingsDevices)
                    {
                        if (!string.IsNullOrEmpty(device.Scope))
                        {
                            if (device.Scope.Contains("metrics"))
                            {
                                FetchWeightLog(device);
                                FetchBloodPressuresLog(device);
                            }
                            if (device.Scope.Contains("activity"))
                            {
                                FetchWorkouts(device);
                                FetchSummaryLog(device);
                                FetchSleepLog(device);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "WithingsManager.FetchWithingsLog", null, ex.Message, null, ex));
            }
        }

        private void FetchSleepLog(UserWearableDeviceDto device)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("action", "getsummary"),
                new KeyValuePair<string, string>("startdate", ((int)(DateTime.UtcNow.AddDays(-7) - unixTime).TotalSeconds).ToString()),
                new KeyValuePair<string, string>("enddatey", ((int)(DateTime.UtcNow - unixTime).TotalSeconds).ToString()),
                });

                WithingsSleep logList = Withings.GetSleep(content, device.Token, ApiUrl);
                if (logList.Status)
                {
                    if (logList.body != null && logList.body.series != null)
                    {
                        foreach (var sleep in logList.body.series)
                            externalReader.AddExtSleep(AddSleep(device.UserId, sleep));
                    }
                }
                else
                {
                    logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "WithingsManager.FetchSleepLog", null, "Request failed : " + logList.Status + " - " + logList.StatusCode + " - " + logList.ErrorMsg, null, logList.Exception));
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "WithingsManager.FetchSleepLog", null, ex.Message, null, ex));
            }
        }

        private static EXT_Sleeps AddSleep(int userId, WithingsSleep.Series request)
        {
            EXT_Sleeps sleep = new EXT_Sleeps();
            sleep.UserId = userId;
            sleep.ExternalId = request.id.ToString();
            sleep.Source = SOURCE;
            sleep.StartTimeStamp = unixTime.AddSeconds(request.created);
            sleep.InputMethod = INPUT_METHOD;
            sleep.IsActive = true;
            sleep.TotalSleepDuration = request.enddate - request.startdate;
            sleep.AwakeDuration = request.data.durationtowakeup;
            sleep.AwakeCount = request.data.wakeupcount;
            sleep.DeepDuration = request.data.deepsleepduration;
            sleep.LightDuration = request.data.lightsleepduration;
            sleep.RemDuration = request.data.remsleepduration;
            sleep.SleepScore = request.data.sleep_score;
            //need to confirm while connect with real users
            //if(request.data.night_events.Count() > 0) 
            //    sleep.TimetoBed = request.data.night_events[0];
            //sleep.TimetoWake = null;
            return sleep;
        }

        private void FetchSummaryLog(UserWearableDeviceDto device)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("action", "getactivity"),
                new KeyValuePair<string, string>("startdateymd", DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-dd")),
                new KeyValuePair<string, string>("enddateymd", DateTime.UtcNow.ToString("yyyy-MM-dd")),
                });

                WithingsSummary logList = Withings.GetSummary(content, device.Token, ApiUrl);

                if (logList.Status)
                {
                    if (logList.body != null && logList.body.activities != null)
                    {
                        foreach (var activity in logList.body.activities)
                        {
                            var summary = AddSummaries(device.UserId, activity);
                            if (summary != null)
                                externalReader.AddExtSummary(summary);
                        }
                    }
                }
                else
                {
                    logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "WithingsManager.FetchActivityLog", null, "Request failed : " + logList.Status + " - " + logList.StatusCode + " - " + logList.ErrorMsg, null, logList.Exception));
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "WithingsManager.FetchActivityLog", null, ex.Message, null, ex));
            }
        }

        private static EXT_Summaries AddSummaries(int userId, WithingsSummary.Activity request)
        {
            EXT_Summaries summary = new EXT_Summaries();
            summary.UserId = userId;
            summary.Source = SOURCE;
            summary.StartTimeStamp = Convert.ToDateTime(request.date);
            summary.EndTimeStamp = summary.StartTimeStamp.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            summary.ExternalId = "withings_" + userId + "_" + summary.StartTimeStamp.ToString("yyyyMMdd");
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(request.timezone));
            summary.StartTimeStamp = TimeZoneInfo.ConvertTimeToUtc(summary.StartTimeStamp, custTZone);
            summary.EndTimeStamp = TimeZoneInfo.ConvertTimeToUtc(summary.EndTimeStamp, custTZone);
            summary.InputMethod = INPUT_METHOD;
            summary.Distance = (float)request.distance;
            summary.ActiveDuration = request.active / 60;
            summary.CaloriesBurned = (float)request.totalcalories;
            summary.Floors = request.elevation;
            summary.Steps = request.steps;
            summary.CaloriesBurnedbyActivity = (float)request.calories;
            if (summary.Distance > 0 || summary.ActiveDuration > 0 || summary.CaloriesBurned > 0 || summary.Floors > 0 || summary.Steps > 0 || summary.CaloriesBurnedbyActivity > 0 || summary.Caloriesbmr > 0)
                return summary;
            else
                return null;
        }

        private void FetchWorkouts(UserWearableDeviceDto device)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("action", "getworkouts"),
                    new KeyValuePair<string, string>("startdateymd", DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-dd")),
                    new KeyValuePair<string, string>("enddateymd", DateTime.UtcNow.ToString("yyyy-MM-dd")),
                });

                WithingsWorkouts logList = Withings.GetWorkouts(content, device.Token, ApiUrl);

                if (logList.Status)
                {
                    if (logList.body != null && logList.body.series != null)
                    {
                        foreach (var workout in logList.body.series)
                            externalReader.AddExtWorkout(AddWorkouts(device.UserId, workout));
                    }
                }
                else
                {
                    logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "WithingsManager.FetchWorkouts", null, "Request failed : " + logList.Status + " - " + logList.StatusCode + " - " + logList.ErrorMsg, null, logList.Exception));
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "WithingsManager.FetchWorkouts", null, ex.Message, null, ex));
            }
        }

        private static EXT_Workouts AddWorkouts(int userId, WithingsWorkouts.Series request)
        {
            EXT_Workouts workout = new EXT_Workouts();
            workout.UserId = userId;
            workout.Source = SOURCE;
            workout.ExternalId = request.id;
            workout.StartTimeStamp = unixTime.AddSeconds(request.startdate);
            workout.EndTimeStamp = unixTime.AddSeconds(request.enddate);
            workout.InputMethod = INPUT_METHOD;
            workout.Category = GetWorkoutType(request.category);
            workout.Duration = request.enddate - request.startdate;
            if (request.data != null && request.data.ToString() != "[]")
            {
                var workoutData = JsonConvert.DeserializeObject<WithingsWorkouts.Data>(request.data.ToString());
                workout.CaloriesBurned = (int)workoutData.calories;
                workout.Distance = (int)workoutData.distance;

            }
            workout.IsActive = true;
            return workout;
        }

        private void FetchBloodPressuresLog(UserWearableDeviceDto device)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("action", "getmeas"),
                    new KeyValuePair<string, string>("meastypes", (int)MeasurementType.SBP +","+ (int)MeasurementType.DBP),
                    new KeyValuePair<string, string>("startdate", ((int)(DateTime.UtcNow.AddDays(-7) - unixTime).TotalSeconds).ToString()),
                    new KeyValuePair<string, string>("enddate", ((int)(DateTime.UtcNow - unixTime).TotalSeconds).ToString()),
                });

                WithingsMeasurements logList = Withings.GetMeasurementsLog(content, device.Token, ApiUrl);

                if (logList.Status)
                {
                    if (logList.body != null && logList.body.measuregrps != null)
                    {
                        foreach (var bloodPressure in logList.body.measuregrps)
                            externalReader.AddExtBloodPressure(AddBloodPressure(device.UserId, bloodPressure));
                    }
                }
                else
                {
                    logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "WithingsManager.FetchBloodPressuresLog", null, "Request failed : " + logList.Status + " - " + logList.StatusCode + " - " + logList.ErrorMsg, null, logList.Exception));
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "WithingsManager.FetchBloodPressuresLog", null, ex.Message, null, ex));
            }
        }

        private static EXT_BloodPressures AddBloodPressure(int userId, WithingsMeasurements.Measuregrp request)
        {
            EXT_BloodPressures bloodPressures = new EXT_BloodPressures();
            bloodPressures.UserId = userId;
            bloodPressures.Source = SOURCE;
            bloodPressures.ExternalId = request.grpid;
            bloodPressures.TimeStamp = unixTime.AddSeconds(request.date);
            bloodPressures.InputMethod = INPUT_METHOD;
            var systolicMeasure = request.measures.Where(x => x.type == (int)MeasurementType.SBP).FirstOrDefault();
            if (systolicMeasure != null)
                bloodPressures.Systolic = (int)(systolicMeasure.value * Math.Pow(10, systolicMeasure.unit));
            var diastolicMeasure = request.measures.Where(x => x.type == (int)MeasurementType.DBP).FirstOrDefault();
            if (diastolicMeasure != null)
                bloodPressures.Diastolic = (int)(diastolicMeasure.value * Math.Pow(10, diastolicMeasure.unit));
            bloodPressures.IsActive = true;
            return bloodPressures;
        }

        private void FetchWeightLog(UserWearableDeviceDto device)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("action", "getmeas"),
                    new KeyValuePair<string, string>("meastypes", (int)MeasurementType.Weight +","+ (int)MeasurementType.FatRatio),
                    new KeyValuePair<string, string>("startdate", ((int)(DateTime.UtcNow.AddDays(-7) - unixTime).TotalSeconds).ToString()),
                    new KeyValuePair<string, string>("enddate", ((int)(DateTime.UtcNow - unixTime).TotalSeconds).ToString()),
                });

                WithingsMeasurements logList = Withings.GetMeasurementsLog(content, device.Token, ApiUrl);

                if (logList.Status)
                {
                    if (logList.body != null && logList.body.measuregrps != null)
                    {
                        foreach (var weight in logList.body.measuregrps)
                            externalReader.AddExtWeight(AddWeightLog(device.UserId, weight), SystemAdminId);
                    }
                }
                else
                {
                    logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "WithingsManager.FetchWeightLog", null, "Request failed : " + logList.Status + " - " + logList.StatusCode + " - " + logList.ErrorMsg, null, logList.Exception));
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "WithingsManager.FetchWeightLog", null, ex.Message, null, ex));
            }
        }

        private static EXT_Weights AddWeightLog(int userId, WithingsMeasurements.Measuregrp request)
        {
            EXT_Weights weight = new EXT_Weights();
            weight.UserId = userId;
            weight.IsActive = true;
            weight.ExternalId = request.grpid;
            weight.TimeStamp = unixTime.AddSeconds(request.date);
            weight.Source = SOURCE;
            weight.InputMethod = INPUT_METHOD;
            var weightMeasure = request.measures.Where(x => x.type == (int)MeasurementType.Weight).FirstOrDefault();
            if (weightMeasure != null)
                weight.Weight = weightMeasure.value * Math.Pow(10, weightMeasure.unit) * 2.205;

            var fatMeasure = request.measures.Where(x => x.type == (int)MeasurementType.FatRatio).FirstOrDefault();
            if (fatMeasure != null)
                weight.FatPercent = fatMeasure.value * Math.Pow(10, fatMeasure.unit);
            return weight;
        }
    }
}
