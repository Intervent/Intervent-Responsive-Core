using Intervent.DAL;
using Intervent.HWS;
using Intervent.HWS.Model;
using Intervent.HWS.Model.Wearable.Garmin;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;

namespace InterventWebApp
{
    public class ExternalUtility
    {
        public static void AddWeight(GarminWeightRequest request, string Source, string inputMethod, int systemAdminId)
        {
            WearableReader _wearableReader = new WearableReader();
            var garminDeviceId = _wearableReader.GetWearableDevices((int)WearableDeviceType.Web).Where(x => x.Name.ToLower() == "garmin").FirstOrDefault().Id;
            ExternalReader externalReader = new ExternalReader();
            foreach (var garminWeight in request.bodyComps)
            {
                EXT_Weights weight = new EXT_Weights();
                var userWearableDevice = _wearableReader.GetAllActiveUserWearableDevices(garminDeviceId).Where(x => x.ExternalUserId == garminWeight.userId).FirstOrDefault();
                if (userWearableDevice != null)
                {
                    weight.UserId = userWearableDevice.UserId;
                    weight.ExternalId = garminWeight.summaryId;
                    weight.TimeStamp = DateTimeOffset.FromUnixTimeSeconds(garminWeight.measurementTimeInSeconds).DateTime;
                    weight.Source = Source;
                    weight.InputMethod = inputMethod;
                    weight.Weight = garminWeight.weightInGrams * 0.00220462;
                    weight.bmi = garminWeight.bodyMassIndex;
                    weight.FatPercent = Convert.ToDouble(garminWeight.bodyFatInPercent);
                    weight.IsActive = true;
                    externalReader.AddExtWeight(weight, systemAdminId);
                }
            }
        }

        public static void AddSleep(GarminSleepRequest request, string Source, string inputMethod)
        {
            WearableReader _wearableReader = new WearableReader();
            var garminDeviceId = _wearableReader.GetWearableDevices((int)WearableDeviceType.Web).Where(x => x.Name.ToLower() == "garmin").FirstOrDefault().Id;
            ExternalReader externalReader = new ExternalReader();
            foreach (var garminSleep in request.sleeps)
            {
                EXT_Sleeps sleep = new EXT_Sleeps();
                var userWearableDevice = _wearableReader.GetAllActiveUserWearableDevices(garminDeviceId).Where(x => x.ExternalUserId == garminSleep.userId).FirstOrDefault();
                if (userWearableDevice != null)
                {
                    sleep.UserId = userWearableDevice.UserId;
                    sleep.ExternalId = garminSleep.summaryId;
                    sleep.Source = Source;
                    sleep.InputMethod = inputMethod;
                    sleep.StartTimeStamp = DateTimeOffset.FromUnixTimeSeconds(garminSleep.startTimeInSeconds).DateTime;
                    sleep.TotalSleepDuration = garminSleep.durationInSeconds;
                    sleep.AwakeDuration = garminSleep.awakeDurationInSeconds;
                    sleep.DeepDuration = garminSleep.deepSleepDurationInSeconds;
                    sleep.LightDuration = garminSleep.lightSleepDurationInSeconds;
                    sleep.RemDuration = garminSleep.remSleepInSeconds;
                    if (garminSleep.overallSleepScore != null)
                        sleep.SleepScore = garminSleep.overallSleepScore.value;
                    sleep.IsActive = true;
                    externalReader.AddExtSleep(sleep);
                }
            }
        }

        public static void AddBloodPressure(GarminBloodPressureRequest request, string Source, string inputMethod)
        {
            WearableReader _wearableReader = new WearableReader();
            var garminDeviceId = _wearableReader.GetWearableDevices((int)WearableDeviceType.Web).Where(x => x.Name.ToLower() == "garmin").FirstOrDefault().Id;
            ExternalReader externalReader = new ExternalReader();
            foreach (var garminBloodPressure in request.bloodPressure)
            {
                EXT_BloodPressures BloodPressure = new EXT_BloodPressures();
                var userWearableDevice = _wearableReader.GetAllActiveUserWearableDevices(garminDeviceId).Where(x => x.ExternalUserId == garminBloodPressure.userId).FirstOrDefault();
                if (userWearableDevice != null)
                {
                    BloodPressure.UserId = userWearableDevice.UserId;
                    BloodPressure.ExternalId = garminBloodPressure.summaryId;
                    BloodPressure.Source = Source;
                    BloodPressure.InputMethod = inputMethod;
                    BloodPressure.IsActive = true;
                    BloodPressure.Systolic = garminBloodPressure.systolic;
                    BloodPressure.Diastolic = garminBloodPressure.diastolic;
                    BloodPressure.TimeStamp = DateTimeOffset.FromUnixTimeSeconds(garminBloodPressure.measurementTimeInSeconds).DateTime;
                    externalReader.AddExtBloodPressure(BloodPressure);
                }
            }
        }

        public static void AddActivity(GarminActivityRequest request, string Source, string inputMethod)
        {
            WearableReader _wearableReader = new WearableReader();
            var garminDeviceId = _wearableReader.GetWearableDevices((int)WearableDeviceType.Web).Where(x => x.Name.ToLower() == "garmin").FirstOrDefault().Id;
            ExternalReader externalReader = new ExternalReader();
            foreach (var activity in request.activities)
            {
                EXT_Workouts workouts = new EXT_Workouts();
                var userWearableDevice = _wearableReader.GetAllActiveUserWearableDevices(garminDeviceId).Where(x => x.ExternalUserId == activity.userId).FirstOrDefault();
                if (userWearableDevice != null)
                {
                    workouts.UserId = userWearableDevice.UserId;
                    workouts.ExternalId = activity.summaryId;
                    workouts.Source = Source;
                    workouts.InputMethod = inputMethod;
                    workouts.IsActive = true;
                    workouts.Name = activity.activityName;
                    workouts.Category = activity.activityType;
                    workouts.StartTimeStamp = DateTimeOffset.FromUnixTimeSeconds(activity.startTimeInSeconds).DateTime;
                    workouts.EndTimeStamp = workouts.StartTimeStamp.Value.AddMilliseconds(activity.durationInSeconds);
                    workouts.Duration = activity.durationInSeconds;
                    workouts.CaloriesBurned = activity.activeKilocalories;
                    workouts.Distance = (float)activity.distanceInMeters;
                    externalReader.AddExtWorkout(workouts);
                }
            }
        }

        public static void AddSummary(GarminDailyDataRequest request, string Source, string inputMethod)
        {
            WearableReader _wearableReader = new WearableReader();
            var garminDeviceId = _wearableReader.GetWearableDevices((int)WearableDeviceType.Web).Where(x => x.Name.ToLower() == "garmin").FirstOrDefault().Id;
            ExternalReader externalReader = new ExternalReader();
            foreach (var dailyData in request.dailies)
            {
                EXT_Summaries summary = new EXT_Summaries();
                var userWearableDevice = _wearableReader.GetAllActiveUserWearableDevices(garminDeviceId).Where(x => x.ExternalUserId == dailyData.userId).FirstOrDefault();
                if (userWearableDevice != null)
                {
                    summary.UserId = userWearableDevice.UserId;
                    summary.ExternalId = dailyData.summaryId;
                    summary.Source = Source;
                    summary.InputMethod = inputMethod;
                    summary.Steps = dailyData.steps;
                    summary.Distance = (float)dailyData.distanceInMeters;
                    summary.StartTimeStamp = DateTimeOffset.FromUnixTimeSeconds(dailyData.startTimeInSeconds).DateTime;
                    summary.EndTimeStamp = DateTimeOffset.FromUnixTimeSeconds(dailyData.startTimeInSeconds + dailyData.durationInSeconds).DateTime;
                    summary.Floors = dailyData.floorsClimbedGoal;
                    summary.Caloriesbmr = dailyData.bmrKilocalories;
                    summary.ActiveDuration = dailyData.activeTimeInSeconds;
                    summary.Distance = (float)dailyData.distanceInMeters;
                    externalReader.AddExtSummary(summary);
                }
            }
        }

        public static void GarminDeregistration(GarminDeregistrationUsers request)
        {
            WearableReader _wearableReader = new WearableReader();
            var garminDeviceId = _wearableReader.GetWearableDevices((int)WearableDeviceType.Web).Where(x => x.Name.ToLower() == "garmin").FirstOrDefault().Id;
            foreach (var garminUser in request.deregistrations)
            {
                var device = _wearableReader.GetAllActiveUserWearableDevices(garminDeviceId).Where(x => x.ExternalUserId == garminUser.userId && x.Token == garminUser.userAccessToken).FirstOrDefault();
                if (device != null)
                {
                    AddOrEditWearableDeviceRequest wearableDeviceRequest = new AddOrEditWearableDeviceRequest
                    {
                        token = device.Token,
                        wearableDeviceId = device.Id,
                        externalUserId = device.ExternalUserId,
                        isActive = false,
                        userId = device.UserId,
                        oauthTokenSecret = device.OauthTokenSecret
                    };
                    _wearableReader.AddOrEditWearableDevice(wearableDeviceRequest);
                }
            }
        }

        public static bool IsValidIntuityUserId(int? extUserId, int? userId)
        {
            ExternalReader reader = new ExternalReader();
            GetIntuityUserRequest request = new GetIntuityUserRequest();
            request.externalUserId = extUserId;
            request.userId = userId;
            return reader.IsValidIntuityUserId(request).isValidIntuityUser;
        }
    }
}