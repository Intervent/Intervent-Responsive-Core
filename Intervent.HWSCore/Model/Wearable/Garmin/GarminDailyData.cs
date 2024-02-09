namespace Intervent.HWS.Model
{
    public class DailyData
    {
        public string userId { get; set; }
        public string userAccessToken { get; set; }
        public string summaryId { get; set; }
        public string calendarDate { get; set; }
        public string activityType { get; set; }
        public int activeKilocalories { get; set; }
        public int bmrKilocalories { get; set; }
        public int steps { get; set; }
        public double distanceInMeters { get; set; }
        public int durationInSeconds { get; set; }
        public int activeTimeInSeconds { get; set; }
        public int startTimeInSeconds { get; set; }
        public int startTimeOffsetInSeconds { get; set; }
        public int moderateIntensityDurationInSeconds { get; set; }
        public int vigorousIntensityDurationInSeconds { get; set; }
        public int floorsClimbed { get; set; }
        public int minHeartRateInBeatsPerMinute { get; set; }
        public int maxHeartRateInBeatsPerMinute { get; set; }
        public int averageHeartRateInBeatsPerMinute { get; set; }
        public int restingHeartRateInBeatsPerMinute { get; set; }
        public int stepsGoal { get; set; }
        public int intensityDurationGoalInSeconds { get; set; }
        public int floorsClimbedGoal { get; set; }
        public int averageStressLevel { get; set; }
        public int maxStressLevel { get; set; }
        public int stressDurationInSeconds { get; set; }
        public int restStressDurationInSeconds { get; set; }
        public int activityStressDurationInSeconds { get; set; }
        public int lowStressDurationInSeconds { get; set; }
        public int mediumStressDurationInSeconds { get; set; }
        public int highStressDurationInSeconds { get; set; }
    }
    public class GarminDailyDataRequest
    {
        public List<DailyData> dailies { get; set; }
    }
}
