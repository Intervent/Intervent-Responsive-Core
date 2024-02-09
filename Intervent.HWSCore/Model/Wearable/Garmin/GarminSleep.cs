namespace Intervent.HWS.Model
{
    public class GarminSleep
    {
        public string userId { get; set; }
        public string userAccessToken { get; set; }
        public string summaryId { get; set; }
        public string calendarDate { get; set; }
        public int durationInSeconds { get; set; }
        public int startTimeInSeconds { get; set; }
        public int startTimeOffsetInSeconds { get; set; }
        public int unmeasurableSleepInSeconds { get; set; }
        public int deepSleepDurationInSeconds { get; set; }
        public int lightSleepDurationInSeconds { get; set; }
        public int remSleepInSeconds { get; set; }
        public int awakeDurationInSeconds { get; set; }
        public string validation { get; set; }
        public TimeOffsetSleepSpo2 timeOffsetSleepSpo2 { get; set; }
        public OverallSleepScore overallSleepScore { get; set; }
        public SleepScores sleepScores { get; set; }
    }
    public class AwakeCount
    {
        public string qualifierKey { get; set; }
    }

    public class DeepPercentage
    {
        public string qualifierKey { get; set; }
    }

    public class LightPercentage
    {
        public string qualifierKey { get; set; }
    }

    public class OverallSleepScore
    {
        public int value { get; set; }
        public string qualifierKey { get; set; }
    }

    public class RemPercentage
    {
        public string qualifierKey { get; set; }
    }

    public class Restlessness
    {
        public string qualifierKey { get; set; }
    }
    public class SleepScores
    {
        public TotalDuration totalDuration { get; set; }
        public Stress stress { get; set; }
        public AwakeCount awakeCount { get; set; }
        public RemPercentage remPercentage { get; set; }
        public Restlessness restlessness { get; set; }
        public LightPercentage lightPercentage { get; set; }
        public DeepPercentage deepPercentage { get; set; }
    }

    public class Stress
    {
        public string qualifierKey { get; set; }
    }

    public class TimeOffsetSleepSpo2
    {
    }

    public class TotalDuration
    {
        public string qualifierKey { get; set; }
    }
    public class GarminSleepRequest
    {
        public List<GarminSleep> sleeps { get; set; }
    }
}
