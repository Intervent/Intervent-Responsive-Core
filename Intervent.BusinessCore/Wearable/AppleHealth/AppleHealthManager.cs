using Intervent.DAL;
using Intervent.HWS.Model;
using Intervent.Web.DataLayer;
using Newtonsoft.Json;
using NLog;
using static Intervent.HWS.Model.AppleHealth;

namespace Intervent.Business
{
    public class AppleHealthManager : BaseManager
    {
        public static LogReader logReader = new LogReader();

        ExternalReader externalReader = new ExternalReader();

        public static string SOURCE = "AppleHealth";

        public static string INPUT_METHOD = "API";

        private static List<EXT_Weights> AddWeight(int userId, AppleHealth.Weight request)
        {
            List<EXT_Weights> list = new List<EXT_Weights>();
            try
            {
                foreach (Dataset data in request.dataset)
                {
                    EXT_Weights weight = new EXT_Weights();
                    weight.UserId = userId;
                    weight.IsActive = true;
                    weight.TimeStamp = data.startDateTime;
                    weight.Source = SOURCE;
                    weight.ExternalId = data.recordId;
                    weight.InputMethod = INPUT_METHOD;
                    weight.Weight = data.record.unit == "Pound" ? Math.Round(data.record.weight.Value, 2) : weight.Weight = Math.Round(data.record.weight.Value * 2.205, 2);
                    if (weight.ExternalId != null)
                        list.Add(weight);
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "AppleHealth.AddWeight", null, "Request : " + JsonConvert.SerializeObject(request) + " - Error : " + ex.Message, null, ex));
            }
            return list;
        }

        private static List<EXT_Glucose> AddGlucose(int userId, BloodGlucose request)
        {
            List<EXT_Glucose> list = new List<EXT_Glucose>();
            try
            {
                AccountReader accountReader = new AccountReader();
                var user = accountReader.GetUserById(userId);
                if (user != null)
                {
                    foreach (Dataset data in request.dataset)
                    {
                        EXT_Glucose glucose = new EXT_Glucose();
                        glucose.UserId = userId;
                        glucose.UniqueId = !string.IsNullOrEmpty(user.UniqueId) ? user.UniqueId : "";
                        glucose.EffectiveDateTime = data.startDateTime;
                        glucose.DateTime = DateTime.UtcNow;
                        glucose.OrganizationId = user.OrganizationId;
                        glucose.ExtId = data.recordId;
                        glucose.Source = (byte)GlucoseSource.AppleHealth;
                        glucose.Code = "2345-7";
                        if (data.record.mealTime != null)
                        {
                            switch (data.record.mealTime)
                            {
                                case "Unspecified": glucose.Code = "2345-7"; break;
                                case "Pre-meal": glucose.Code = "53049-3"; break;
                                case "Post-meal": glucose.Code = "1521-4"; break;
                            }
                        }
                        glucose.Unit = data.record.unit;
                        glucose.Value = glucose.Unit == "mg/dL" ? data.record.glucose.Value : data.record.glucose.Value / 18;
                        glucose.IsValid = glucose.Value > 0;
                        if (glucose.ExtId != null)
                            list.Add(glucose);
                    }
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "AppleHealthManager.AddGlucose", null, "Request : " + JsonConvert.SerializeObject(request) + " - Error : " + ex.Message, null, ex));
            }
            return list;
        }

        private static List<EXT_BloodPressures> AddBloodPressures(int userId, BloodPressure request)
        {
            List<EXT_BloodPressures> list = new List<EXT_BloodPressures>();
            try
            {
                foreach (Dataset data in request.dataset)
                {
                    EXT_BloodPressures bloodpressure = new EXT_BloodPressures();
                    bloodpressure.UserId = userId;
                    bloodpressure.IsActive = true;
                    bloodpressure.TimeStamp = data.startDateTime;
                    bloodpressure.ExternalId = data.recordId;
                    bloodpressure.Source = SOURCE;
                    bloodpressure.InputMethod = INPUT_METHOD;
                    if (data.record.diastolic.HasValue)
                        bloodpressure.Diastolic = data.record.diastolic.Value;
                    if (data.record.systolic.HasValue)
                        bloodpressure.Systolic = data.record.systolic.Value;
                    if (bloodpressure.ExternalId != null)
                        list.Add(bloodpressure);
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "AppleHealthManager.AddBloodPressures", null, "Request : " + JsonConvert.SerializeObject(request) + " - Error : " + ex.Message, null, ex));
            }
            return list;
        }

        private static List<EXT_Summaries> AddSummaries(int userId, string type, string timeZoneName, List<EXT_Summaries> list, List<Dataset> request)
        {
            try
            {
                TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
                foreach (Dataset point in request)
                {
                    DateTime date = TimeZoneInfo.ConvertTimeToUtc(point.startDateTime.Date, custTZone);
                    var extId = "apple_" + userId + "_" + date.ToString("yyyyMMdd");

                    EXT_Summaries summary = list.Where(x => x.ExternalId == extId).FirstOrDefault();
                    if (summary == null)
                    {
                        summary = new EXT_Summaries();
                        summary.UserId = userId;
                        summary.Source = SOURCE;
                        summary.StartTimeStamp = date;
                        summary.EndTimeStamp = date.AddHours(23).AddMinutes(59).AddSeconds(59);
                        summary.ExternalId = extId;
                        summary.InputMethod = INPUT_METHOD;
                        list.Add(summary);
                    }

                    if (type.Equals("caloriesBurned"))
                        summary.CaloriesBurned = summary.CaloriesBurned.HasValue ? (float)Math.Round(summary.CaloriesBurned.Value + point.record.caloriesBurned.Value, 2) : (float)Math.Round((float)point.record.caloriesBurned.Value, 2);
                    else if (type.Equals("steps"))
                        summary.Steps = summary.Steps.HasValue ? summary.Steps.Value + point.record.steps.Value : point.record.steps.Value;
                    else if (type.Equals("distance"))
                        summary.Distance = summary.Distance.HasValue ? (float)Math.Round(summary.Distance.Value + (float)point.record.distance.Value, 2) : (float)Math.Round((float)point.record.distance.Value, 2);
                    else if (type.Equals("activeEnergyBurned"))
                        summary.CaloriesBurnedbyActivity = summary.CaloriesBurnedbyActivity.HasValue ? (summary.CaloriesBurnedbyActivity.Value + (int)point.record.activeEnergyBurned.Value) : (int)point.record.activeEnergyBurned.Value;
                    else if (type.Equals("water"))
                        summary.Water = summary.Water.HasValue ? (float)Math.Round(summary.Water.Value + point.record.water.Value, 2) : (float)Math.Round(point.record.water.Value, 2);
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "AppleHealthManager.AddSummaries", null, "Request : " + JsonConvert.SerializeObject(request) + " - Error : " + ex.Message, null, ex));
            }
            return list;
        }

        private static List<EXT_Sleeps> AddSleep(int userId, AppleHealth.Sleep request)
        {
            List<EXT_Sleeps> list = new List<EXT_Sleeps>();
            try
            {
                foreach (Dataset data in request.dataset)
                {
                    bool old = false;
                    EXT_Sleeps sleep = null;
                    if (list.Where(x => x.StartTimeStamp.Date == data.startDateTime.Date || (x.StartTimeStamp < data.startDateTime && x.StartTimeStamp > data.startDateTime.AddHours(-9))).Any())
                    {
                        old = true;
                        sleep = list.Where(x => x.StartTimeStamp.Date == data.startDateTime.Date || (x.StartTimeStamp < data.startDateTime && x.StartTimeStamp > data.startDateTime.AddHours(-9))).FirstOrDefault();
                    }
                    else
                    {
                        sleep = new EXT_Sleeps();
                        sleep.UserId = userId;
                        sleep.Source = SOURCE;
                        sleep.StartTimeStamp = Convert.ToDateTime(data.startDateTime);
                        sleep.InputMethod = INPUT_METHOD;
                        sleep.IsActive = true;
                        sleep.ExternalId = "apple_" + userId + "_" + data.startDateTime.ToString("yyyyMMdd");
                    }
                    switch (data.record.sleep)
                    {
                        case "Awake": // Awake; user is awake.
                            int awake = (int)(data.endDateTime - data.startDateTime).TotalSeconds;
                            sleep.AwakeDuration = sleep.AwakeDuration.HasValue ? sleep.AwakeDuration.Value + awake : awake;
                            sleep.AwakeCount = sleep.AwakeCount.HasValue ? sleep.AwakeCount.Value + 1 : 1;
                            break;
                        case "Asleep": // Sleeping; generic or non-granular sleep description.
                            int asleep = (int)(data.endDateTime - data.startDateTime).TotalSeconds;
                            sleep.TotalSleepDuration = sleep.TotalSleepDuration.HasValue ? sleep.TotalSleepDuration.Value + asleep : asleep;
                            break;
                        case "Core": // Light sleep; user is in a light sleep cycle.
                            int light = (int)(data.endDateTime - data.startDateTime).TotalSeconds;
                            sleep.LightDuration = sleep.LightDuration.HasValue ? sleep.LightDuration.Value + light : light;
                            sleep.TotalSleepDuration = sleep.TotalSleepDuration.HasValue ? sleep.TotalSleepDuration.Value + light : light;
                            break;
                        case "Deep": // Deep sleep; user is in a deep sleep cycle.
                            int deep = (int)(data.endDateTime - data.startDateTime).TotalSeconds;
                            sleep.DeepDuration = sleep.DeepDuration.HasValue ? sleep.DeepDuration.Value + deep : deep;
                            sleep.TotalSleepDuration = sleep.TotalSleepDuration.HasValue ? sleep.TotalSleepDuration.Value + deep : deep;
                            break;
                        case "REM": // REM sleep; user is in a REM sleep cyle.
                            int rem = (int)(data.endDateTime - data.startDateTime).TotalSeconds;
                            sleep.RemDuration = sleep.RemDuration.HasValue ? sleep.RemDuration.Value + rem : rem;
                            sleep.TotalSleepDuration = sleep.TotalSleepDuration.HasValue ? sleep.TotalSleepDuration.Value + rem : rem;
                            break;
                    }
                    if (!old)
                        list.Add(sleep);
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "AppleHealthManager.AddBloodPressures", null, "Request : " + JsonConvert.SerializeObject(request) + " - Error : " + ex.Message, null, ex));
            }
            return list;
        }

        private static List<EXT_Workouts> AddFitness(int userId, Activity request)
        {
            List<EXT_Workouts> workoutList = new List<EXT_Workouts>();
            foreach (var activity in request.dataset)
            {
                EXT_Workouts workout = new EXT_Workouts();
                workout.UserId = userId;
                workout.Source = SOURCE;
                workout.StartTimeStamp = activity.startDateTime;
                workout.EndTimeStamp = activity.endDateTime;
                workout.ExternalId = activity.recordId;
                workout.Name = activity.record.activityType;
                workout.Category = activity.record.activityType;
                workout.InputMethod = INPUT_METHOD;
                workout.IsActive = true;
                if (activity.record.totalEnergyBurned.HasValue)
                    workout.CaloriesBurned = (int)activity.record.totalEnergyBurned.Value;
                if (activity.record.distance.HasValue)
                    workout.Distance = (float)activity.record.totalDistance.Value;
                if (activity.record.duration.HasValue)
                    workout.Duration = (float)activity.record.duration.Value / 60;
                workoutList.Add(workout);
            }
            return workoutList;
        }

        private static List<EXT_Nutrition> AddNutrition(int userId, string type, string timeZoneName, List<EXT_Nutrition> list, List<Dataset> request)
        {
            try
            {
                TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
                foreach (Dataset food in request)
                {
                    DateTime date = TimeZoneInfo.ConvertTimeToUtc(food.startDateTime.Date, custTZone);
                    var extId = "apple_" + userId + "_" + date.ToString("yyyyMMdd");

                    EXT_Nutrition nutrition = list.Where(x => x.ExternalId == extId).FirstOrDefault();
                    if (nutrition == null)
                    {
                        nutrition = new EXT_Nutrition();
                        nutrition.UserId = userId;
                        nutrition.TimeStamp = date;
                        nutrition.ExternalId = extId;
                        nutrition.Source = SOURCE;
                        nutrition.Meal = "Unknown";
                        nutrition.Name = "Unknown";
                        nutrition.Last_updated = DateTime.UtcNow.ToString();
                        nutrition.InputMethod = INPUT_METHOD;
                        nutrition.IsActive = true;
                        nutrition.Validated = true;
                        nutrition.Servings = 1;
                        nutrition.ServingUnit = "gram";
                        list.Add(nutrition);
                    }
                    if (type.Equals("dietryEnergy") && food.record.dietryEnergy.HasValue)
                        nutrition.Calories = nutrition.Calories.HasValue ? nutrition.Calories + food.record.dietryEnergy.Value : food.record.dietryEnergy.Value;
                    if (type.Equals("carbohydrate") && food.record.carbohydrate.HasValue)
                        nutrition.Carbohydrates = nutrition.Carbohydrates.HasValue ? nutrition.Carbohydrates + food.record.carbohydrate : food.record.carbohydrate;
                    if (type.Equals("fat") && food.record.fat.HasValue)
                        nutrition.Fat = nutrition.Fat.HasValue ? nutrition.Fat + food.record.fat.Value : food.record.fat.Value;
                    if (type.Equals("fiber") && food.record.fiber.HasValue)
                        nutrition.Fiber = nutrition.Fiber.HasValue ? nutrition.Fiber + food.record.fiber.Value : food.record.fiber.Value;
                    if (type.Equals("protein") && food.record.protein.HasValue)
                        nutrition.Protein = nutrition.Protein.HasValue ? nutrition.Protein + food.record.protein.Value : food.record.protein.Value;
                    if (type.Equals("sodium") && food.record.sodium.HasValue)
                        nutrition.Sodium = nutrition.Sodium.HasValue ? Math.Round((double)(nutrition.Sodium + food.record.sodium.Value), 2) : Math.Round(food.record.sodium.Value, 2);
                    if (type.Equals("water") && food.record.water.HasValue)
                        nutrition.Water = nutrition.Water.HasValue ? Math.Round((double)(nutrition.Water + food.record.water.Value), 2) : Math.Round(food.record.water.Value, 2);
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "AppleHealthManager.AddNutrition", null, "Request : " + JsonConvert.SerializeObject(request) + " - Error : " + ex.Message, null, ex));
            }
            return list;
        }

        public void ProcessAppleHealthdata(int userId, string timeZoneName, AppleHealth request, int systemAdminId)
        {
            try
            {
                List<EXT_Summaries> eXT_Summaries = new List<EXT_Summaries>();
                List<EXT_Nutrition> eXT_Nutrition = new List<EXT_Nutrition>();
                if (request.weight != null && request.weight.dataset.Count() > 0)
                {
                    var list = AddWeight(userId, request.weight);
                    foreach (var data in list)
                        externalReader.AddExtWeight(data, systemAdminId);
                }
                if (request.bloodGlucose != null && request.bloodGlucose.dataset.Count() > 0)
                {
                    var list = AddGlucose(userId, request.bloodGlucose);
                    foreach (var data in list)
                        externalReader.AddGlucose(data);
                }
                if (request.bloodPressure != null && request.bloodPressure.dataset.Count() > 0)
                {
                    var list = AddBloodPressures(userId, request.bloodPressure);
                    foreach (var data in list)
                        externalReader.AddExtBloodPressure(data);
                }
                if (request.sleep != null && request.sleep.dataset.Count() > 0)
                {
                    var list = AddSleep(userId, request.sleep);
                    foreach (var data in list)
                        externalReader.AddExtSleep(data);
                }
                if (request.activity != null && request.activity.dataset.Count() > 0)
                {
                    var list = AddFitness(userId, request.activity);
                    foreach (var data in list)
                        externalReader.AddExtWorkout(data);
                }
                if (request.fat != null && request.fat.dataset.Count() > 0)
                {
                    eXT_Nutrition = AddNutrition(userId, "fat", timeZoneName, eXT_Nutrition, request.fat.dataset);
                }
                if (request.dietryEnergy != null && request.dietryEnergy.dataset != null && request.dietryEnergy.dataset.Count() > 0)
                {
                    eXT_Nutrition = AddNutrition(userId, "dietryEnergy", timeZoneName, eXT_Nutrition, request.dietryEnergy.dataset);
                }
                if (request.carbohydrate != null && request.carbohydrate.dataset.Count() > 0)
                {
                    eXT_Nutrition = AddNutrition(userId, "carbohydrate", timeZoneName, eXT_Nutrition, request.carbohydrate.dataset);
                }
                if (request.fiber != null && request.fiber.dataset.Count() > 0)
                {
                    eXT_Nutrition = AddNutrition(userId, "fiber", timeZoneName, eXT_Nutrition, request.fiber.dataset);
                }
                if (request.protein != null && request.protein.dataset.Count() > 0)
                {
                    eXT_Nutrition = AddNutrition(userId, "protein", timeZoneName, eXT_Nutrition, request.protein.dataset);
                }
                if (request.sodium != null && request.sodium.dataset.Count() > 0)
                {
                    eXT_Nutrition = AddNutrition(userId, "sodium", timeZoneName, eXT_Nutrition, request.sodium.dataset);
                }
                if (request.water != null && request.water.dataset.Count() > 0)
                {
                    eXT_Nutrition = AddNutrition(userId, "water", timeZoneName, eXT_Nutrition, request.water.dataset);
                    eXT_Summaries = AddSummaries(userId, "water", timeZoneName, eXT_Summaries, request.water.dataset);
                }
                if (request.steps != null && request.steps.dataset.Count() > 0)
                {
                    eXT_Summaries = AddSummaries(userId, "steps", timeZoneName, eXT_Summaries, request.steps.dataset);
                }
                if (request.caloriesBurned != null && request.caloriesBurned.dataset.Count() > 0)
                {
                    eXT_Summaries = AddSummaries(userId, "caloriesBurned", timeZoneName, eXT_Summaries, request.caloriesBurned.dataset);
                }
                if (request.summary != null && request.summary.dataset.Count() > 0)
                {
                    eXT_Summaries = AddSummaries(userId, "activeEnergyBurned", timeZoneName, eXT_Summaries, request.summary.dataset);
                }
                if (request.distance != null && request.distance.dataset.Count() > 0)
                {
                    eXT_Summaries = AddSummaries(userId, "distance", timeZoneName, eXT_Summaries, request.distance.dataset);
                }
                if (eXT_Summaries.Count() > 0)
                    foreach (var data in eXT_Summaries)
                        externalReader.AddExtSummary(data);
                if (eXT_Nutrition.Count() > 0)
                    externalReader.BulkAddExtNutrition(eXT_Nutrition);
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "AppleHealthManager.ProcessAppleHealthdata", null, ex.Message, null, ex));
            }
        }
    }
}
