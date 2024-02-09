using Intervent.DAL;
using Intervent.HWS;
using Intervent.HWS.Model;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Newtonsoft.Json;
using NLog;
using System.Configuration;
using static Intervent.HWS.Model.GoogleDataSet;

namespace Intervent.Business
{
    public class GoogleFitManager : BaseManager
    {
        public static LogReader logReader = new LogReader();

        ExternalReader externalReader = new ExternalReader();

        public static string SOURCE = "GoogleFit";

        public static string INPUT_METHOD = "API";

        public static DateTime currentTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        public static string ClientId = ConfigurationManager.AppSettings["GoogleFitClientId"];

        public static string ClientSecret = ConfigurationManager.AppSettings["GoogleFitClientSecret"];

        public GoogleFitManager() { }

        public GoogleFitManager(int systemAdminId)
        {
            SystemAdminId = systemAdminId;
        }

        private static string GetWorkoutType(int workoutType)
        {
            if (workoutType == 9)
                return "Aerobics";
            else if (workoutType == 119)
                return "Archery";
            else if (workoutType == 10)
                return "Badminton";
            else if (workoutType == 11)
                return "Baseball";
            else if (workoutType == 12)
                return "Basketball";
            else if (workoutType == 12)
                return "Biathlon";
            else if (workoutType == 1)
                return "Biking";
            else if (workoutType == 14)
                return "Handbiking";
            else if (workoutType == 15)
                return "Mountain biking";
            else if (workoutType == 16)
                return "Road biking";
            else if (workoutType == 17)
                return "Spinning";
            else if (workoutType == 18)
                return "Stationary biking";
            else if (workoutType == 19)
                return "Utility biking";
            else if (workoutType == 20)
                return "Boxing";
            else if (workoutType == 21)
                return "Calisthenics";
            else if (workoutType == 22)
                return "Circuit training";
            else if (workoutType == 23)
                return "Cricket";
            else if (workoutType == 113)
                return "Crossfit";
            else if (workoutType == 106)
                return "Curling";
            else if (workoutType == 24)
                return "Dancing";
            else if (workoutType == 102)
                return "Diving";
            else if (workoutType == 117)
                return "Elevator";
            else if (workoutType == 25)
                return "Elliptical";
            else if (workoutType == 103)
                return "Ergometer";
            else if (workoutType == 118)
                return "Escalator";
            else if (workoutType == 26)
                return "Fencing";
            else if (workoutType == 27)
                return "Football (American)";
            else if (workoutType == 28)
                return "Football (Australian)";
            else if (workoutType == 29)
                return "Football (Soccer)";
            else if (workoutType == 30)
                return "Frisbee";
            else if (workoutType == 31)
                return "Gardening";
            else if (workoutType == 32)
                return "Golf";
            else if (workoutType == 122)
                return "Guided Breathing";
            else if (workoutType == 33)
                return "Gymnastics";
            else if (workoutType == 34)
                return "Handball";
            else if (workoutType == 114)
                return "HIIT";
            else if (workoutType == 35)
                return "Hiking";
            else if (workoutType == 36)
                return "Hockey";
            else if (workoutType == 37)
                return "Horseback riding";
            else if (workoutType == 38)
                return "Housework";
            else if (workoutType == 104)
                return "Ice skating";
            else if (workoutType == 115)
                return "Interval Training";
            else if (workoutType == 39)
                return "Jumping rope";
            else if (workoutType == 40)
                return "Kayaking";
            else if (workoutType == 41)
                return "Kettlebell training";
            else if (workoutType == 42)
                return "Kickboxing";
            else if (workoutType == 43)
                return "Kitesurfing";
            else if (workoutType == 44)
                return "Martial arts";
            else if (workoutType == 45)
                return "Meditation";
            else if (workoutType == 46)
                return "Mixed martial arts";
            else if (workoutType == 108)
                return "Other (unclassified fitness activity)";
            else if (workoutType == 47)
                return "P90X exercises";
            else if (workoutType == 48)
                return "Paragliding";
            else if (workoutType == 49)
                return "Pilates";
            else if (workoutType == 50)
                return "Polo";
            else if (workoutType == 51)
                return "Racquetball";
            else if (workoutType == 52)
                return "Rock climbing";
            else if (workoutType == 53)
                return "Rowing";
            else if (workoutType == 54)
                return "Rowing machine";
            else if (workoutType == 55)
                return "Rugby";
            else if (workoutType == 8)
                return "Running";
            else if (workoutType == 56)
                return "Jogging";
            else if (workoutType == 57)
                return "Running on sand";
            else if (workoutType == 58)
                return "Running (treadmill)";
            else if (workoutType == 59)
                return "Sailing";
            else if (workoutType == 60)
                return "Scuba diving";
            else if (workoutType == 61)
                return "Skateboarding";
            else if (workoutType == 62)
                return "Skating";
            else if (workoutType == 63)
                return "Cross skating";
            else if (workoutType == 105)
                return "Indoor skating";
            else if (workoutType == 64)
                return "Inline skating (rollerblading)";
            else if (workoutType == 65)
                return "Skiing";
            else if (workoutType == 66)
                return "Back-country skiing";
            else if (workoutType == 67)
                return "Cross-country skiing";
            else if (workoutType == 68)
                return "Downhill skiing";
            else if (workoutType == 69)
                return "Kite skiing";
            else if (workoutType == 70)
                return "Roller skiing";
            else if (workoutType == 71)
                return "Sledding";
            else if (workoutType == 73)
                return "Snowboarding";
            else if (workoutType == 74)
                return "Snowmobile";
            else if (workoutType == 75)
                return "Snowshoeing";
            else if (workoutType == 120)
                return "Softball";
            else if (workoutType == 76)
                return "Squash";
            else if (workoutType == 77)
                return "Stair climbing";
            else if (workoutType == 78)
                return "Stair-climbing machine";
            else if (workoutType == 79)
                return "Stand-up paddleboarding";
            else if (workoutType == 3)
                return "Still (not moving)";
            else if (workoutType == 80)
                return "Strength training";
            else if (workoutType == 81)
                return "Surfing";
            else if (workoutType == 82)
                return "Swimming";
            else if (workoutType == 84)
                return "Swimming (open water)";
            else if (workoutType == 83)
                return "Swimming (swimming pool)";
            else if (workoutType == 85)
                return "Table tennis (ping pong)";
            else if (workoutType == 86)
                return "Team sports";
            else if (workoutType == 87)
                return "Tennis";
            else if (workoutType == 5)
                return "Tilting (sudden device gravity change)";
            else if (workoutType == 88)
                return "Treadmill (walking or running)";
            else if (workoutType == 4)
                return "Unknown (unable to detect activity)";
            else if (workoutType == 89)
                return "Volleyball";
            else if (workoutType == 90)
                return "Volleyball (beach)";
            else if (workoutType == 91)
                return "Volleyball (indoor)";
            else if (workoutType == 92)
                return "Wakeboarding";
            else if (workoutType == 7)
                return "Walking";
            else if (workoutType == 93)
                return "Walking (fitness)";
            else if (workoutType == 94)
                return "Nording walking";
            else if (workoutType == 95)
                return "Walking (treadmill)";
            else if (workoutType == 116)
                return "Walking (stroller)";
            else if (workoutType == 96)
                return "Waterpolo";
            else if (workoutType == 97)
                return "Weightlifting";
            else if (workoutType == 98)
                return "Wheelchair";
            else if (workoutType == 99)
                return "Windsurfing";
            else if (workoutType == 100)
                return "Yoga";
            else if (workoutType == 101)
                return "Zumba";
            else
                return "Unknown";
        }

        public static string GetMealType(int mealType)
        {
            if (mealType == 0)
                return "Unknown";
            else if (mealType == 1)
                return "Breakfast";
            else if (mealType == 2)
                return "Lunch";
            else if (mealType == 3)
                return "Dinner";
            else if (mealType == 4)
                return "Snack";
            else
                return "Unknown";
        }

        private static List<EXT_Weights> AddWeight(int userId, GoogleFitness.Dataset request)
        {
            List<EXT_Weights> list = new List<EXT_Weights>();
            try
            {
                foreach (GoogleFitness.Point point in request.point)
                {
                    long nanos = Convert.ToInt64(point.startTimeNanos) / 100;
                    EXT_Weights weight = new EXT_Weights();
                    weight.UserId = userId;
                    weight.IsActive = true;
                    weight.TimeStamp = currentTime.AddTicks(nanos);
                    weight.Source = SOURCE;
                    weight.ExternalId = point.startTimeNanos.ToString();
                    weight.InputMethod = INPUT_METHOD;
                    weight.Weight = Math.Round(point.value.FirstOrDefault().fpVal.Value * 2.205, 2);
                    list.Add(weight);
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "GoogleFitManager.AddWeight", null, "Request : " + JsonConvert.SerializeObject(request) + " - Error : " + ex.Message, null, ex));
            }
            return list;
        }

        private static List<EXT_Weights> AddBodyFat(int userId, GoogleFitness.Dataset request)
        {
            List<EXT_Weights> list = new List<EXT_Weights>();
            try
            {
                foreach (GoogleFitness.Point point in request.point)
                {
                    if (point.value.FirstOrDefault().fpVal.HasValue)
                    {
                        long nanos = Convert.ToInt64(point.startTimeNanos) / 100;
                        EXT_Weights weight = new EXT_Weights();
                        weight.UserId = userId;
                        weight.TimeStamp = currentTime.AddTicks(nanos);
                        weight.FatPercent = point.value.FirstOrDefault().fpVal.Value;
                        weight.Source = SOURCE;
                        weight.InputMethod = INPUT_METHOD;
                        list.Add(weight);
                    }
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "GoogleFitManager.AddBodyFat", null, "Request : " + JsonConvert.SerializeObject(request) + " - Error : " + ex.Message, null, ex));
            }
            return list;
        }

        private static List<EXT_Glucose> AddGlucose(int userId, GoogleFitness.Dataset request)
        {
            List<EXT_Glucose> list = new List<EXT_Glucose>();
            try
            {
                AccountReader accountReader = new AccountReader();
                var user = accountReader.GetUserById(userId);
                if (user != null)
                {
                    foreach (GoogleFitness.Point point in request.point)
                    {
                        EXT_Glucose glucose = new EXT_Glucose();
                        long nanos = Convert.ToInt64(point.startTimeNanos) / 100;
                        glucose.UserId = userId;
                        glucose.UniqueId = !string.IsNullOrEmpty(user.UniqueId) ? user.UniqueId : "";
                        glucose.EffectiveDateTime = currentTime.AddTicks(nanos);
                        glucose.DateTime = DateTime.UtcNow;
                        glucose.OrganizationId = user.OrganizationId;
                        glucose.ExtId = point.startTimeNanos.ToString();
                        glucose.Source = (byte)GlucoseSource.GoogleFit;
                        glucose.Code = "2345-7";
                        if (point.value[0].fpVal.HasValue)
                        {
                            switch (point.value[0].fpVal.Value)
                            {
                                case 1: glucose.Code = "2345-7"; break;
                                case 2: glucose.Code = "1558-6"; break;
                                case 3: glucose.Code = "53049-3"; break;
                                case 4: glucose.Code = "1521-4"; break;
                            }
                        }
                        glucose.Value = ((int)point.value[0].fpVal.Value) * 18;
                        glucose.Unit = "mg/dL";
                        glucose.IsValid = glucose.Value > 0;
                        list.Add(glucose);
                    }
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "GoogleFitManager.AddGlucose", null, "Request : " + JsonConvert.SerializeObject(request) + " - Error : " + ex.Message, null, ex));
            }
            return list;
        }

        private static List<EXT_BloodPressures> AddBloodPressures(int userId, GoogleFitness.Dataset request)
        {
            List<EXT_BloodPressures> list = new List<EXT_BloodPressures>();
            try
            {
                foreach (GoogleFitness.Point point in request.point)
                {
                    EXT_BloodPressures bloodpressure = new EXT_BloodPressures();
                    long nanos = Convert.ToInt64(point.startTimeNanos) / 100;
                    bloodpressure.UserId = userId;
                    bloodpressure.IsActive = true;
                    bloodpressure.TimeStamp = currentTime.AddTicks(nanos);
                    bloodpressure.ExternalId = point.startTimeNanos.ToString();
                    bloodpressure.Source = SOURCE;
                    bloodpressure.InputMethod = INPUT_METHOD;
                    if (point.value[1].fpVal.HasValue)
                        bloodpressure.Diastolic = (int)point.value[1].fpVal.Value;
                    if (point.value[0].fpVal.HasValue)
                        bloodpressure.Systolic = (int)point.value[0].fpVal.Value;
                    list.Add(bloodpressure);
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "GoogleFitManager.AddBloodPressures", null, "Request : " + JsonConvert.SerializeObject(request) + " - Error : " + ex.Message, null, ex));
            }
            return list;
        }

        private static List<EXT_BloodPressures> AddHeartMinutes(int userId, GoogleFitness.Dataset request)
        {
            List<EXT_BloodPressures> finalList = new List<EXT_BloodPressures>();
            try
            {
                List<EXT_BloodPressures> list = new List<EXT_BloodPressures>();
                foreach (GoogleFitness.Point point in request.point)
                {
                    EXT_BloodPressures bloodpressure = new EXT_BloodPressures();
                    long nanos = Convert.ToInt64(point.startTimeNanos) / 100;
                    bloodpressure.UserId = userId;
                    bloodpressure.TimeStamp = currentTime.AddTicks(nanos);
                    if (point.value[0].fpVal.HasValue)
                        bloodpressure.RestingHeartRate = (int)point.value[0].fpVal.Value;
                    list.Add(bloodpressure);
                }
                foreach (var date in list.Select(x => x.TimeStamp.Date).Distinct())
                {
                    EXT_BloodPressures bloodpressure = new EXT_BloodPressures();
                    int bpm = (int)list.Where(x => x.TimeStamp.Date == date).Average(y => y.RestingHeartRate.Value);
                    bloodpressure.UserId = userId;
                    bloodpressure.TimeStamp = date;
                    bloodpressure.Source = SOURCE;
                    bloodpressure.InputMethod = INPUT_METHOD;
                    bloodpressure.RestingHeartRate = bpm;
                    finalList.Add(bloodpressure);
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "GoogleFitManager.AddHeartMinutes", null, "Request : " + JsonConvert.SerializeObject(request) + " - Error : " + ex.Message, null, ex));
            }
            return finalList;
        }

        private static List<EXT_Nutrition> AddNutrition(int userId, GoogleFitness.Dataset request)
        {
            List<EXT_Nutrition> list = new List<EXT_Nutrition>();
            try
            {
                foreach (GoogleFitness.Point point in request.point)
                {
                    long nanos = Convert.ToInt64(point.startTimeNanos) / 100;
                    EXT_Nutrition nutrition = new EXT_Nutrition();
                    nutrition.UserId = userId;
                    nutrition.TimeStamp = currentTime.AddTicks(nanos);
                    nutrition.ExternalId = point.startTimeNanos.ToString();
                    nutrition.Source = SOURCE;
                    nutrition.Last_updated = DateTime.UtcNow.ToString();
                    nutrition.InputMethod = INPUT_METHOD;
                    nutrition.IsActive = true;
                    nutrition.Validated = true;
                    nutrition.ServingUnit = "gram";
                    nutrition.Servings = 1;

                    if (point.value[0].intVal.HasValue)
                        nutrition.Meal = GetMealType(point.value[0].intVal.Value);
                    if (!string.IsNullOrEmpty(point.value[1].stringVal))
                        nutrition.Name = point.value[1].stringVal;
                    if (point.value[0].mapVal.Count() > 0)
                    {
                        if (point.value[0].mapVal.Where(x => x.key == "fat.total").Any())
                            nutrition.Fat = point.value[0].mapVal.Where(x => x.key == "fat.total").FirstOrDefault().value.fpVal;
                        if (point.value[0].mapVal.Where(x => x.key == "protein").Any())
                            nutrition.Protein = point.value[0].mapVal.Where(x => x.key == "protein").FirstOrDefault().value.fpVal;
                        if (point.value[0].mapVal.Where(x => x.key == "carbs.total").Any())
                            nutrition.Carbohydrates = point.value[0].mapVal.Where(x => x.key == "carbs.total").FirstOrDefault().value.fpVal;
                        if (point.value[0].mapVal.Where(x => x.key == "calories").Any())
                            nutrition.Calories = point.value[0].mapVal.Where(x => x.key == "calories").FirstOrDefault().value.fpVal;
                        if (point.value[0].mapVal.Where(x => x.key == "dietary_fiber").Any())
                            nutrition.Fiber = point.value[0].mapVal.Where(x => x.key == "dietary_fiber").FirstOrDefault().value.fpVal;
                        if (point.value[0].mapVal.Where(x => x.key == "sodium").Any())
                            nutrition.Sodium = point.value[0].mapVal.Where(x => x.key == "sodium").FirstOrDefault().value.fpVal;
                    }
                    list.Add(nutrition);
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "GoogleFitManager.AddNutrition", null, "Request : " + JsonConvert.SerializeObject(request) + " - Error : " + ex.Message, null, ex));
            }
            return list;
        }

        private static List<EXT_Sleeps> AddSleep(int userId, long tstartDate, long tendDate, GoogleFitness.Dataset request)
        {
            List<EXT_Sleeps> list = new List<EXT_Sleeps>();
            try
            {
                if (request.point == null)
                {
                    long awake = Convert.ToInt64(tendDate) - Convert.ToInt64(tstartDate);
                    EXT_Sleeps sleep = new EXT_Sleeps
                    {
                        UserId = userId,
                        Source = SOURCE,
                        StartTimeStamp = currentTime.AddMilliseconds(tstartDate),
                        InputMethod = INPUT_METHOD,
                        IsActive = true,
                        ExternalId = tstartDate.ToString(),
                        TotalSleepDuration = Convert.ToInt32(awake / 1000)
                    };
                    list.Add(sleep);
                }
                else
                {
                    foreach (GoogleFitness.Point point in request.point)
                    {
                        bool old = false;
                        long nanos = Convert.ToInt64(point.startTimeNanos) / 100;
                        DateTime startDate = currentTime.AddTicks(nanos);

                        EXT_Sleeps sleep = null;
                        if (list.Where(x => x.StartTimeStamp.Date == startDate.Date || (x.StartTimeStamp < startDate && x.StartTimeStamp > startDate.AddHours(-9))).Any())
                        {
                            old = true;
                            sleep = list.Where(x => x.StartTimeStamp.Date == startDate.Date || (x.StartTimeStamp < startDate && x.StartTimeStamp > startDate.AddHours(-9))).FirstOrDefault();
                        }
                        else
                        {
                            sleep = new EXT_Sleeps();
                            sleep.UserId = userId;
                            sleep.Source = SOURCE;
                            sleep.StartTimeStamp = startDate;
                            sleep.InputMethod = INPUT_METHOD;
                            sleep.IsActive = true;
                            sleep.ExternalId = point.startTimeNanos;
                        }

                        switch (point.value[0].intVal)
                        {
                            case 1: // Awake; user is awake.
                                long awake = Convert.ToInt64(point.endTimeNanos) - Convert.ToInt64(point.startTimeNanos);
                                sleep.AwakeDuration = sleep.AwakeDuration.HasValue ? sleep.AwakeDuration.Value + Convert.ToInt32(awake / 1e+9) : Convert.ToInt32(awake / 1e+9);
                                sleep.AwakeCount = sleep.AwakeCount.HasValue ? sleep.AwakeCount.Value + 1 : 1;
                                break;
                            case 2: // Sleeping; generic or non-granular sleep description.
                                long generic = Convert.ToInt64(point.endTimeNanos) - Convert.ToInt64(point.startTimeNanos);
                                sleep.TotalSleepDuration = sleep.TotalSleepDuration.HasValue ? sleep.TotalSleepDuration.Value + Convert.ToInt32(generic / 1e+9) : Convert.ToInt32(generic / 1e+9);
                                break;
                            case 3: // Out of bed; user gets out of bed in the middle of a sleep session.
                                sleep.WakeCount = sleep.WakeCount.HasValue ? sleep.WakeCount.Value + 1 : 1;
                                break;
                            case 4: // Light sleep; user is in a light sleep cycle.
                                long light = Convert.ToInt64(point.endTimeNanos) - Convert.ToInt64(point.startTimeNanos);
                                sleep.LightDuration = sleep.LightDuration.HasValue ? sleep.LightDuration.Value + Convert.ToInt32(light / 1e+9) : Convert.ToInt32(light / 1e+9);
                                sleep.TotalSleepDuration = sleep.TotalSleepDuration.HasValue ? sleep.TotalSleepDuration.Value + Convert.ToInt32(light / 1e+9) : Convert.ToInt32(light / 1e+9);
                                break;
                            case 5: // Deep sleep; user is in a deep sleep cycle.
                                long deep = Convert.ToInt64(point.endTimeNanos) - Convert.ToInt64(point.startTimeNanos);
                                sleep.DeepDuration = sleep.DeepDuration.HasValue ? sleep.DeepDuration.Value + Convert.ToInt32(deep / 1e+9) : Convert.ToInt32(deep / 1e+9);
                                sleep.TotalSleepDuration = sleep.TotalSleepDuration.HasValue ? sleep.TotalSleepDuration.Value + Convert.ToInt32(deep / 1e+9) : Convert.ToInt32(deep / 1e+9);
                                break;
                            case 6: // REM sleep; user is in a REM sleep cyle.
                                long rem = Convert.ToInt64(point.endTimeNanos) - Convert.ToInt64(point.startTimeNanos);
                                sleep.RemDuration = sleep.RemDuration.HasValue ? sleep.RemDuration.Value + Convert.ToInt32(rem / 1e+9) : Convert.ToInt32(rem / 1e+9);
                                sleep.TotalSleepDuration = sleep.TotalSleepDuration.HasValue ? sleep.TotalSleepDuration.Value + Convert.ToInt32(rem / 1e+9) : Convert.ToInt32(rem / 1e+9);
                                break;
                        }
                        if (!old)
                            list.Add(sleep);
                    }
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "GoogleFitManager.AddSleep", null, "Request : " + JsonConvert.SerializeObject(request) + " - Error : " + ex.Message, null, ex));
            }
            return list;
        }

        private static List<EXT_Summaries> AddSummaries(int userId, List<EXT_Summaries> list, GoogleFitness.Dataset request, string startTimeNanos, string endTimeNanos)
        {
            try
            {
                foreach (GoogleFitness.Point point in request.point)
                {
                    if (point.value.Count() > 0)
                    {
                        long startTime = Convert.ToInt64(startTimeNanos) / 1000;
                        long endTime = Convert.ToInt64(endTimeNanos) / 1000;

                        var extId = "google_" + userId + "_" + currentTime.AddSeconds(startTime).ToString("yyyyMMdd");

                        EXT_Summaries summary = list.Where(x => x.ExternalId == extId).FirstOrDefault();
                        if (summary == null)
                        {
                            summary = new EXT_Summaries();
                            summary.UserId = userId;
                            summary.Source = SOURCE;
                            summary.StartTimeStamp = currentTime.AddSeconds(startTime);
                            summary.EndTimeStamp = currentTime.AddSeconds(endTime);
                            summary.ExternalId = extId;
                            summary.InputMethod = INPUT_METHOD;
                            list.Add(summary);
                        }

                        if (point.dataTypeName.Contains("com.google.calories.expended"))
                            summary.CaloriesBurned = summary.CaloriesBurned.HasValue ? (float)Math.Round(summary.CaloriesBurned.Value + (float)point.value[0].fpVal.Value, 2) : (float)Math.Round((float)point.value[0].fpVal.Value, 2);
                        else if (point.dataTypeName.Contains("com.google.calories.bmr"))
                            summary.Caloriesbmr = summary.Caloriesbmr.HasValue ? (float)Math.Round(summary.Caloriesbmr.Value + (float)point.value[0].fpVal.Value, 2) : (float)Math.Round((float)point.value[0].fpVal.Value, 2);
                        else if (point.dataTypeName.Contains("com.google.step_count.delta"))
                            summary.Steps = summary.Steps.HasValue ? summary.Steps.Value + point.value[0].intVal.Value : point.value[0].intVal.Value;
                        else if (point.dataTypeName.Contains("com.google.distance.delta"))
                            summary.Distance = summary.Distance.HasValue ? (float)Math.Round(summary.Distance.Value + (float)point.value[0].fpVal.Value, 2) : (float)Math.Round((float)point.value[0].fpVal.Value, 2);
                        else if (point.dataTypeName.Contains("com.google.active_minutes"))
                            summary.ActiveDuration = summary.ActiveDuration.HasValue ? summary.ActiveDuration.Value + point.value[0].intVal.Value : point.value[0].intVal.Value;
                        else if (point.dataTypeName.Contains("com.google.hydration"))
                            summary.Water = summary.Water.HasValue ? summary.Water.Value + (float)point.value[0].fpVal.Value : (float)point.value[0].fpVal.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "GoogleFitManager.AddSummaries", null, "Request : " + JsonConvert.SerializeObject(request) + " - Error : " + ex.Message, null, ex));
            }
            return list;
        }

        private static List<EXT_Workouts> AddFitness(int userId, List<EXT_Workouts> list, GoogleFitness.Dataset request)
        {
            try
            {
                foreach (GoogleFitness.Point point in request.point)
                {
                    if (point.value.Count() > 0)
                    {
                        string name = GetWorkoutType(point.value[0].intVal.Value);
                        if (name != "Unknown")
                        {
                            EXT_Workouts workout = list.Where(x => x.ExternalId == point.startTimeNanos).FirstOrDefault();
                            if (workout == null)
                            {
                                workout = new EXT_Workouts();
                                long startTime = Convert.ToInt64(point.startTimeNanos) / 100;
                                long endTime = Convert.ToInt64(point.endTimeNanos) / 100;
                                workout.UserId = userId;
                                workout.Source = SOURCE;
                                workout.StartTimeStamp = currentTime.AddTicks(startTime);
                                workout.EndTimeStamp = currentTime.AddTicks(endTime);
                                workout.ExternalId = point.startTimeNanos;
                                workout.InputMethod = INPUT_METHOD;
                                workout.IsActive = true;
                                list.Add(workout);
                            }
                            if (point.dataTypeName.Contains("com.google.activity.segment"))
                            {
                                workout.Name = name;
                                workout.Category = point.value[0].stringVal;
                                workout.Duration = (float)Math.Round((workout.EndTimeStamp - workout.StartTimeStamp).Value.TotalSeconds, 0);
                            }
                            if (point.dataTypeName.Contains("com.google.calories.expended"))
                                workout.CaloriesBurned = workout.CaloriesBurned.HasValue ? Convert.ToInt32(workout.CaloriesBurned.Value + point.value[0].fpVal.Value) : Convert.ToInt32(point.value[0].fpVal.Value);
                            if (point.dataTypeName.Contains("com.google.distance.delta"))
                                workout.Distance = workout.Distance.HasValue ? workout.Distance.Value + (float)point.value[0].fpVal.Value : (float)point.value[0].fpVal.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "GoogleFitManager.AddFitness", null, "Request : " + JsonConvert.SerializeObject(request) + " - Error : " + ex.Message, null, ex));
            }
            return list;
        }

        public void UpdateAccesstoken(IList<UserWearableDeviceDto> devices)
        {
            try
            {
                GoogleFitClient client = new GoogleFitClient(null);
                foreach (var device in devices)
                {
                    GoogleFitOAuth oAuth = Task.Run(() => client.RefreshTokenAsync(device.UserId, device.RefreshToken, device.ExternalUserId, device.WearableDeviceId, true, ClientId, ClientSecret)).Result;
                    if (!oAuth.Status && oAuth.StatusCode != System.Net.HttpStatusCode.OK)
                        logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "GoogleFitManager.UpdateAccesstoken", null, "Refresh token failed for User : " + device.UserId + ". ExternalUserId : " + device.ExternalUserId + ", Status code : " + oAuth.StatusCode, null, null));
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "GoogleFitManager.UpdateAccesstoken", null, ex.Message, null, ex));
            }
        }

        public void FetchGoogleFitLog(int deviceId, IList<UserWearableDeviceDto> googleFitDevice)
        {
            try
            {
                UpdateAccesstoken(googleFitDevice);

                WearableReader wearableReader = new WearableReader();
                var activeUsersGoogleFitDevices = wearableReader.GetAllActiveUserWearableDevices(deviceId);
                if (activeUsersGoogleFitDevices.Count > 0)
                {
                    logReader.WriteLogMessage(new LogEventInfo(LogLevel.Trace, "GoogleFitManager.FetchGoogleFitLog", null, "GoogleFit device count (" + activeUsersGoogleFitDevices.Count() + ").", null, null));
                    foreach (var device in activeUsersGoogleFitDevices)
                    {
                        FetchFitnessLog(device);
                    }
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "GoogleFitManager.FetchGoogleFitLog", null, ex.Message, null, ex));
            }
        }

        private void FetchFitnessLog(UserWearableDeviceDto device)
        {
            try
            {
                GoogleDataSet request = new GoogleDataSet();
                request.aggregateBy = new List<AggregateBy>();
                request.startTimeMillis = (long)(DateTime.UtcNow.Date.AddDays(-7) - new DateTime(1970, 1, 1)).TotalMilliseconds;
                request.endTimeMillis = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

                AggregateBy weight = new AggregateBy();
                weight.dataTypeName = "com.google.weight";
                request.aggregateBy.Add(weight);

                AggregateBy fat = new AggregateBy();
                fat.dataTypeName = "com.google.body.fat.percentage";
                //request.aggregateBy.Add(fat);

                AggregateBy bp = new AggregateBy();
                bp.dataTypeName = "com.google.blood_pressure";
                request.aggregateBy.Add(bp);

                AggregateBy heart_minutes = new AggregateBy();
                heart_minutes.dataTypeName = "com.google.heart_rate.bpm";
                //request.aggregateBy.Add(heart_minutes);

                AggregateBy glucose = new AggregateBy();
                glucose.dataTypeName = "com.google.blood_glucose";
                request.aggregateBy.Add(glucose);

                AggregateBy nutrition = new AggregateBy();
                nutrition.dataTypeName = "com.google.nutrition";
                request.aggregateBy.Add(nutrition);

                AggregateBy sleep = new AggregateBy();
                sleep.dataTypeName = "com.google.sleep.segment";
                request.aggregateBy.Add(sleep);

                AggregateBy step = new AggregateBy();
                step.dataTypeName = "com.google.step_count.delta";
                request.aggregateBy.Add(step);

                AggregateBy calories = new AggregateBy();
                calories.dataTypeName = "com.google.calories.expended";
                request.aggregateBy.Add(calories);

                AggregateBy distance = new AggregateBy();
                distance.dataTypeName = "com.google.distance.delta";
                request.aggregateBy.Add(distance);

                AggregateBy active_minutes = new AggregateBy();
                active_minutes.dataTypeName = "com.google.active_minutes";
                request.aggregateBy.Add(active_minutes);

                AggregateBy water = new AggregateBy();
                water.dataTypeName = "com.google.hydration";
                request.aggregateBy.Add(water);

                string timeZoneName = "UTC";
                if (device.User.TimeZoneId.HasValue)
                {
                    IList<TimeZoneDto> TimeZones = new CommonReader().GetTimeZones(new ReadTimeZonesRequest { Id = device.User.TimeZoneId.Value }).TimeZones;
                    timeZoneName = TimeZones[0].TimeZoneId;
                }

                GoogleFitness fitnessLog = GoogleFit.GetFitnessDetails(device.Token, request);
                ProcessGoogleFitnessdata(device.UserId, fitnessLog);
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "GoogleFitManager.FetchFitnessLog", null, ex.Message, null, ex));
            }
        }

        public void ProcessGoogleFitnessdata(int userId, GoogleFitness fitnessLog)
        {
            try
            {
                if (fitnessLog != null && fitnessLog.Status)
                {
                    if (fitnessLog.bucket.Count() > 0)
                    {
                        foreach (var bucket in fitnessLog.bucket)
                        {
                            List<EXT_Summaries> eXT_Summaries = new List<EXT_Summaries>();
                            List<EXT_Workouts> eXT_Workouts = new List<EXT_Workouts>();
                            var tstartDate = Convert.ToInt64(bucket.startTimeMillis);
                            var tendDate = Convert.ToInt64(bucket.endTimeMillis);

                            foreach (var log in bucket.dataset)
                            {
                                if (log.dataSourceId.Contains("com.google.sleep.segment") || log.point != null && log.point.Count() > 0)
                                {
                                    if (log.dataSourceId.Contains("com.google.weight"))
                                    {
                                        var list = AddWeight(userId, log);
                                        foreach (var data in list)
                                            externalReader.AddExtWeight(data, SystemAdminId);
                                    }
                                    else if (log.dataSourceId.Contains("com.google.body.fat.percentage"))
                                    {
                                        var list = AddBodyFat(userId, log);
                                        foreach (var data in list)
                                            externalReader.UpdateBodyFatToExtWeight(data);
                                    }
                                    else if (log.dataSourceId.Contains("com.google.blood_glucose"))
                                    {
                                        var list = AddGlucose(userId, log);
                                        foreach (var data in list)
                                            externalReader.AddGlucose(data);
                                    }
                                    else if (log.dataSourceId.Contains("com.google.blood_pressure"))
                                    {
                                        var list = AddBloodPressures(userId, log);
                                        foreach (var data in list)
                                            externalReader.AddExtBloodPressure(data);
                                    }
                                    else if (log.dataSourceId.Contains("com.google.heart_minutes"))
                                    {
                                        var list = AddHeartMinutes(userId, log);
                                        foreach (var data in list)
                                            externalReader.UpdateHeartRateToExtBloodPressure(data);
                                    }
                                    else if (log.dataSourceId.Contains("com.google.nutrition"))
                                    {
                                        var list = AddNutrition(userId, log);
                                        if (list.Count() > 0)
                                            externalReader.BulkAddExtNutrition(list);
                                    }
                                    else if (log.dataSourceId.Contains("com.google.sleep.segment"))
                                    {
                                        var list = AddSleep(userId, tstartDate, tendDate, log);
                                        foreach (var data in list)
                                            if (data.TotalSleepDuration.HasValue && data.TotalSleepDuration.Value > 0)
                                                externalReader.AddExtSleep(data);
                                    }
                                    else if (!log.dataSourceId.Contains("aggregated")
                                        && (log.dataSourceId.Contains("com.google.activity.segment")
                                        || log.dataSourceId.Contains("com.google.step_count.delta")
                                        || log.dataSourceId.Contains("com.google.calories.expended")
                                        || log.dataSourceId.Contains("com.google.distance.delta")
                                        || log.dataSourceId.Contains("com.google.power.sample")
                                        || log.dataSourceId.Contains("com.google.cycling.pedaling.cadence")
                                        || log.dataSourceId.Contains("com.google.cycling.wheel_revolution.cumulative")))
                                    {
                                        eXT_Workouts = AddFitness(userId, eXT_Workouts, log);
                                    }
                                    else if (log.dataSourceId.Contains("aggregated"))
                                    {
                                        eXT_Summaries = AddSummaries(userId, eXT_Summaries, log, bucket.startTimeMillis, bucket.endTimeMillis);
                                    }
                                }
                            }
                            if (eXT_Summaries.Count() > 0)
                                foreach (var data in eXT_Summaries)
                                    externalReader.AddExtSummary(data);

                            if (eXT_Workouts.Count() > 0)
                                foreach (var data in eXT_Workouts)
                                    if (!string.IsNullOrEmpty(data.Name))
                                        externalReader.AddExtWorkout(data);
                        }
                    }
                }
                else
                {
                    logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "GoogleFitManager.ProcessGoogleFitnessdata", null, "Request failed : " + fitnessLog.Status + " - " + fitnessLog.StatusCode + " - " + fitnessLog.ErrorMsg + " - Body : " + JsonConvert.SerializeObject(fitnessLog.error), null, fitnessLog.Exception));
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "GoogleFitManager.ProcessGoogleFitnessdata", null, ex.Message, null, ex));
            }
        }
    }
}
