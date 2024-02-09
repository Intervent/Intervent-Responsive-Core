using Intervent.DAL;
using Intervent.HWS;
using Intervent.HWS.Model;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using System.Configuration;

namespace Intervent.Business
{
    public class FitbitManager : BaseManager
    {
        WearableReader _wearableReader = new WearableReader();

        LogReader logReader = new LogReader();

        ExternalReader externalReader = new ExternalReader();

        public static string SOURCE = "Fitbit";

        public static string INPUT_METHOD = "API";

        public static string ApiUrl = ConfigurationManager.AppSettings["FitbitApiUrl"];

        public static string ClientId = ConfigurationManager.AppSettings["FitbitClientId"];

        public static string ClientSecret = ConfigurationManager.AppSettings["FitbitClientSecret"];

        public static string GetMealType(int mealType)
        {
            if (mealType == 1)
                return "Breakfast";
            else if (mealType == 2)
                return "Morning Snack";
            else if (mealType == 3)
                return "Lunch";
            else if (mealType == 4)
                return "Afternoon Snack";
            else if (mealType == 5)
                return "Dinner";
            else if (mealType == 6)
                return "After Dinner";
            else if (mealType == 7)
                return "Anytime";
            else
                return "N/A";
        }
        
        private static EXT_Weights AddWeight(int userId, HWS.Model.Weight request)
        {
            EXT_Weights weight = new EXT_Weights();
            weight.UserId = userId;
            weight.IsActive = true;
            weight.TimeStamp = Convert.ToDateTime(request.date);
            weight.Source = SOURCE;
            weight.ExternalId = request.logId.ToString();
            weight.InputMethod = INPUT_METHOD;
            weight.Weight = request.weight;
            weight.bmi = request.bmi;
            if (request.fat.HasValue)
                weight.FatPercent = Convert.ToDouble(request.fat.Value);
            return weight;
        }

        private static List<EXT_Nutrition> AddNutrition(int userId, FitbitFood request, string OffsetFromUTC)
        {
            List<EXT_Nutrition> nutritionsList = new List<EXT_Nutrition>();

            foreach (var food in request.foods)
            {
                EXT_Nutrition nutrition = new EXT_Nutrition();
                nutrition.UserId = userId;
                nutrition.TimeStamp = Convert.ToDateTime(food.logDate);
                if (!string.IsNullOrEmpty(OffsetFromUTC))
                    nutrition.Utc_offset = OffsetFromUTC;
                nutrition.ExternalId = food.logId.ToString();
                nutrition.Source = SOURCE;
                nutrition.Meal = GetMealType(food.loggedFood.mealTypeId);
                nutrition.Name = food.loggedFood.name;
                nutrition.Last_updated = DateTime.UtcNow.ToString();
                nutrition.InputMethod = INPUT_METHOD;
                nutrition.IsActive = true;
                nutrition.Validated = true;
                if (food.loggedFood.amount != 0)
                    nutrition.Servings = food.loggedFood.amount;
                nutrition.ServingUnit = food.loggedFood.unit.name;
                if (food.loggedFood.calories != 0)
                    nutrition.Calories = food.loggedFood.calories;
                if (food.nutritionalValues != null)
                {
                    if (food.nutritionalValues.carbs != 0)
                        nutrition.Carbohydrates = food.nutritionalValues.carbs;
                    if (food.nutritionalValues.fat != 0)
                        nutrition.Fat = food.nutritionalValues.fat;
                    if (food.nutritionalValues.fiber != 0)
                        nutrition.Fiber = food.nutritionalValues.fiber;
                    if (food.nutritionalValues.protein != 0)
                        nutrition.Protein = food.nutritionalValues.protein;
                    if (food.nutritionalValues.sodium != 0)
                        nutrition.Sodium = food.nutritionalValues.sodium;
                }
                nutritionsList.Add(nutrition);
            }
            return nutritionsList;
        }

        private static EXT_Sleeps AddSleep(int userId, Sleep request, int offsetFromUTC)
        {
            EXT_Sleeps sleep = new EXT_Sleeps();
            sleep.UserId = userId;
            sleep.ExternalId = request.logId.ToString();
            sleep.Source = SOURCE;
            sleep.StartTimeStamp = Convert.ToDateTime(request.startTime).AddMilliseconds(-(offsetFromUTC)); ;
            sleep.InputMethod = INPUT_METHOD;
            sleep.IsActive = true;
            if (request.minutesAsleep != 0)
                sleep.TotalSleepDuration = request.minutesAsleep * 60;
            if (request.minutesAwake != 0)
                sleep.AwakeDuration = request.minutesAwake * 60;
            if (request.levels.summary.wake != null)
                sleep.WakeCount = request.levels.summary.wake.count;
            if (request.levels.summary.awake != null)
                sleep.AwakeCount = request.levels.summary.awake.count;
            if (request.levels.summary.deep != null)
                sleep.DeepDuration = request.levels.summary.deep.minutes * 60;
            if (request.levels.summary.light != null)
                sleep.LightDuration = request.levels.summary.light.minutes * 60;
            if (request.levels.summary.rem != null)
                sleep.RemDuration = request.levels.summary.rem.minutes * 60;
            if (request.minutesToFallAsleep != 0)
                sleep.TimetoBed = request.minutesToFallAsleep;
            if (request.minutesAfterWakeup != 0)
                sleep.TimetoWake = request.minutesAfterWakeup;
            if (request.efficiency != 0)
                sleep.SleepScore = request.efficiency;
            return sleep;
        }

        private static List<EXT_Workouts> AddFitness(int userId, FitbitActivitySummary request, int offsetFromUTC)
        {
            List<EXT_Workouts> workoutList = new List<EXT_Workouts>();
            float miles = (float)1609.344;
            foreach (var activity in request.activities)
            {
                EXT_Workouts workout = new EXT_Workouts();
                workout.UserId = userId;
                workout.Source = SOURCE;
                workout.StartTimeStamp = Convert.ToDateTime(activity.startDate + " " + activity.startTime).AddMilliseconds(-(offsetFromUTC));
                workout.EndTimeStamp = workout.StartTimeStamp.Value.AddMilliseconds(activity.duration);
                workout.ExternalId = activity.logId.ToString();
                workout.Name = activity.name;
                workout.Category = activity.activityParentName;
                workout.InputMethod = INPUT_METHOD;
                workout.IsActive = true;
                if (activity.calories != 0)
                    workout.CaloriesBurned = activity.calories;
                if (activity.distance.HasValue)
                    workout.Distance = (float)activity.distance * miles;
                if (activity.duration != 0)
                    workout.Duration = (float)activity.duration / 1000;
                workoutList.Add(workout);
            }
            return workoutList;
        }

        private static EXT_Summaries AddSummaries(int userId, FitbitActivitySummary request, DateTime date, int offsetFromUTC)
        {
            float miles = (float)1609.344;
            EXT_Summaries summary = new EXT_Summaries();
            summary.UserId = userId;
            summary.Source = SOURCE;
            summary.StartTimeStamp = date.Date.Date.AddMilliseconds(-(offsetFromUTC));
            summary.EndTimeStamp = date.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(-(offsetFromUTC));
            summary.ExternalId = "fitbit_" + userId + "_" + date.ToString("yyyyMMdd");
            summary.InputMethod = INPUT_METHOD;
            if (request.summary.distances.Count() > 0)
                summary.Distance = (float)request.summary.distances.Where(x => x.activity == "total").FirstOrDefault().distance * miles;
            if (request.summary != null)
            {
                summary.ActiveDuration = request.summary.fairlyActiveMinutes * 60 + request.summary.veryActiveMinutes * 60;
            }
            if (request.summary.caloriesOut != 0)
                summary.CaloriesBurned = request.summary.caloriesOut;
            if (request.summary.floors != 0)
                summary.Floors = request.summary.floors;
            if (request.summary.steps != 0)
                summary.Steps = request.summary.steps;
            if (request.summary.activityCalories != 0)
                summary.CaloriesBurnedbyActivity = request.summary.activityCalories;
            if (request.summary.caloriesBMR != 0)
                summary.Caloriesbmr = request.summary.caloriesBMR;
            if (summary.Distance > 0 || summary.ActiveDuration > 0 || summary.CaloriesBurned > 0 || summary.Floors > 0 || summary.Steps > 0 || summary.CaloriesBurnedbyActivity > 0 || summary.Caloriesbmr > 0 || summary.Water > 0)
                return summary;
            else
                return null;
        }

        public void FetchFitBitLog(int deviceId, IList<UserWearableDeviceDto> usersFitbitDevices)
        {
            try
            {
                UpdateAccesstoken(usersFitbitDevices);

                var activeUsersFitbitDevices = _wearableReader.GetAllActiveUserWearableDevices(deviceId);
                if (activeUsersFitbitDevices.Count > 0)
                {
                    logReader.WriteLogMessage(new LogEventInfo(LogLevel.Trace, "FitbitManager.FetchFitBitLog", null, "FetchFitBitLog - device count (" + activeUsersFitbitDevices.Count() + ").", null, null));
                    foreach (var device in activeUsersFitbitDevices)
                    {
                        if (!string.IsNullOrEmpty(device.Scope))
                        {
                            if (device.Scope.Contains("nutrition"))
                                FetchFoodLog(device);
                            if (device.Scope.Contains("weight"))
                                FetchWeightLog(device);
                            if (device.Scope.Contains("sleep"))
                                FetchSleepLog(device);
                            if (device.Scope.Contains("activity"))
                                FetchActivitySummaryLog(device);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "FitbitManager.FetchFitBitLog", null, ex.Message, null, ex));
            }
        }

        public void FetchFoodLog(UserWearableDeviceDto device)
        {
            try
            {
                for (int i = 0; i < 7; i++)
                {
                    DateTime date = DateTime.UtcNow.AddDays(-i);
                    if (device.OffsetFromUTC.HasValue)
                        date = date.AddMilliseconds(-(device.OffsetFromUTC.Value));
                    FitbitFood foodList = Fitbit.GetFood(device.Token, device.ExternalUserId, date, ApiUrl);
                    if (foodList.Status)
                    {
                        if (foodList.foods != null)
                            externalReader.BulkAddExtNutrition(AddNutrition(device.UserId, foodList, device.OffsetFromUTC.ToString()));
                    }
                    else
                    {
                        logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "FitbitManager.FetchFoodLog", null, "Request failed : " + foodList.Status + " - " + foodList.StatusCode + " - " + foodList.ErrorMsg, null, foodList.Exception));
                    }
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "FitbitManager.FetchFoodLog", null, ex.Message, null, ex));
            }
        }

        public void FetchWeightLog(UserWearableDeviceDto device)
        {
            try
            {
                FitbitWeight weightList = Fitbit.GetWeight(device.Token, device.ExternalUserId, ApiUrl);
                if (weightList.Status)
                {
                    if (weightList.weight.Count() > 0)
                    {
                        foreach (var weight in weightList.weight)
                            externalReader.AddExtWeight(AddWeight(device.UserId, weight), SystemAdminId);
                    }
                }
                else
                {
                    logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "FitbitManager.FetchWeightLog", null, "Request failed : " + weightList.Status + " - " + weightList.StatusCode + " - " + weightList.ErrorMsg, null, weightList.Exception));
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "FitbitManager.FetchWeightLog", null, ex.Message, null, ex));
            }
        }

        public void FetchSleepLog(UserWearableDeviceDto device)
        {
            try
            {
                for (int i = 0; i < 7; i++)
                {
                    int offsetFromUTC = device.OffsetFromUTC.HasValue ? device.OffsetFromUTC.Value : 0;
                    DateTime date = DateTime.UtcNow.AddDays(-i).AddMilliseconds(-(offsetFromUTC));
                    FitbitSleep logList = Fitbit.GetSleep(device.Token, device.ExternalUserId, date, ApiUrl);
                    if (logList.Status)
                    {
                        if (logList.sleep != null)
                        {
                            foreach (var sleep in logList.sleep)
                                externalReader.AddExtSleep(AddSleep(device.UserId, sleep, offsetFromUTC));
                        }
                    }
                    else
                    {
                        logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "FitbitManager.FetchSleepLog", null, "Request failed : " + logList.Status + " - " + logList.StatusCode + " - " + logList.ErrorMsg, null, logList.Exception));
                    }
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "FitbitManager.FetchSleepLog", null, ex.Message, null, ex));
            }
        }

        public void FetchActivitySummaryLog(UserWearableDeviceDto device)
        {
            try
            {
                for (int i = 0; i < 7; i++)
                {
                    int offsetFromUTC = device.OffsetFromUTC.HasValue ? device.OffsetFromUTC.Value : 0;
                    DateTime date = DateTime.UtcNow.AddDays(-i).AddMilliseconds(-(offsetFromUTC));
                    FitbitActivitySummary logList = Fitbit.GetActivitySummary(device.Token, device.ExternalUserId, date, ApiUrl);
                    if (logList.Status)
                    {
                        if (logList.summary != null)
                        {
                            var summary = AddSummaries(device.UserId, logList, date, offsetFromUTC);
                            if (summary != null)
                                externalReader.AddExtSummary(summary);

                            if (logList.activities.Count() > 0)
                            {
                                List<EXT_Workouts> workouts = AddFitness(device.UserId, logList, offsetFromUTC);
                                foreach (var workout in workouts)
                                    externalReader.AddExtWorkout(workout);
                            }
                        }
                    }
                    else
                    {
                        logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "FitbitManager.FetchActivitySummaryLog", null, "Request failed : " + logList.Status + " - " + logList.StatusCode + " - " + logList.ErrorMsg, null, logList.Exception));
                    }
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "FitbitManager.FetchActivitySummaryLog", null, ex.Message, null, ex));
            }
        }

        public void UpdateAccesstoken(IList<UserWearableDeviceDto> devices)
        {
            try
            {
                FitbitClient client = new FitbitClient();
                foreach (var device in devices)
                {
                    FitbitOAuth2 oAuth = Task.Run(() => client.RefreshTokenAsync(device.UserId, device.RefreshToken, device.ExternalUserId, device.WearableDeviceId, true, ApiUrl, ClientId, ClientSecret)).Result;
                    if (!oAuth.Status && oAuth.StatusCode != System.Net.HttpStatusCode.OK)
                        logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "FitbitManager.UpdateAccesstoken", null, "Refresh token failed for User : " + device.UserId + ". ExternalUserId : " + device.ExternalUserId + ", Status code : " + oAuth.StatusCode, null, null));
                }
            }
            catch (Exception ex)
            {
                logReader.WriteLogMessage(new LogEventInfo(LogLevel.Error, "FitbitManager.UpdateAccesstoken", null, ex.Message, null, ex));
            }
        }
    }
}
