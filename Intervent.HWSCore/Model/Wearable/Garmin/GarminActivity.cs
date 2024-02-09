namespace Intervent.HWS.Model
{
    public class GarminActivity
    {
        public string userId { get; set; }
        public string userAccessToken { get; set; }
        public string summaryId { get; set; }
        public long activityId { get; set; }
        public string activityName { get; set; }
        public string activityDescription { get; set; }
        public int durationInSeconds { get; set; }
        public int startTimeInSeconds { get; set; }
        public int startTimeOffsetInSeconds { get; set; }
        public string activityType { get; set; }
        public int averageHeartRateInBeatsPerMinute { get; set; }
        public double averageRunCadenceInStepsPerMinute { get; set; }
        public double averageSpeedInMetersPerSecond { get; set; }
        public double averagePaceInMinutesPerKilometer { get; set; }
        public int activeKilocalories { get; set; }
        public double distanceInMeters { get; set; }
        public int maxHeartRateInBeatsPerMinute { get; set; }
        public double maxPaceInMinutesPerKilometer { get; set; }
        public double maxRunCadenceInStepsPerMinute { get; set; }
        public double maxSpeedInMetersPerSecond { get; set; }
        public int steps { get; set; }
        public double totalElevationGainInMeters { get; set; }
    }
    public class GarminActivityRequest
    {
        public List<GarminActivity> activities { get; set; }
    }
}
