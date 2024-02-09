namespace Intervent.HWS.Model
{
    public class FitbitSleep : ProcessResponse
    {
        public List<Sleep> sleep { get; set; }

        public Summary summary { get; set; }
    }

    public class Datum
    {
        public DateTime dateTime { get; set; }

        public string level { get; set; }

        public int seconds { get; set; }
    }

    public class Deep
    {
        public int count { get; set; }

        public int minutes { get; set; }

        public int thirtyDayAvgMinutes { get; set; }
    }

    public class Levels
    {
        public List<Datum> data { get; set; }

        public List<ShortDatum> shortData { get; set; }

        public Summary summary { get; set; }
    }

    public class Light
    {
        public int count { get; set; }

        public int minutes { get; set; }

        public int thirtyDayAvgMinutes { get; set; }
    }

    public class Rem
    {
        public int count { get; set; }

        public int minutes { get; set; }

        public int thirtyDayAvgMinutes { get; set; }
    }

    public class ShortDatum
    {
        public DateTime dateTime { get; set; }

        public string level { get; set; }

        public int seconds { get; set; }
    }

    public class Sleep
    {
        public string dateOfSleep { get; set; }

        public int duration { get; set; }

        public int efficiency { get; set; }

        public DateTime endTime { get; set; }

        public int infoCode { get; set; }

        public bool isMainSleep { get; set; }

        public Levels levels { get; set; }

        public long logId { get; set; }

        public int minutesAfterWakeup { get; set; }

        public int minutesAsleep { get; set; }

        public int minutesAwake { get; set; }

        public int minutesToFallAsleep { get; set; }

        public string logType { get; set; }

        public DateTime startTime { get; set; }

        public int timeInBed { get; set; }

        public string type { get; set; }
    }

    public class Stages
    {
        public int deep { get; set; }

        public int light { get; set; }

        public int rem { get; set; }

        public int wake { get; set; }
    }

    public class Summary
    {
        public Deep deep { get; set; }

        public Light light { get; set; }

        public Rem rem { get; set; }

        public Wake wake { get; set; }

        public Stages stages { get; set; }

        public int totalMinutesAsleep { get; set; }

        public int totalSleepRecords { get; set; }

        public int totalTimeInBed { get; set; }

        public Asleep asleep { get; set; }

        public Awake awake { get; set; }

        public Restless restless { get; set; }
    }

    public class Wake
    {
        public int count { get; set; }

        public int minutes { get; set; }

        public int thirtyDayAvgMinutes { get; set; }
    }

    public class Asleep
    {
        public int count { get; set; }

        public int minutes { get; set; }
    }

    public class Awake
    {
        public int count { get; set; }

        public int minutes { get; set; }
    }

    public class Restless
    {
        public int count { get; set; }

        public int minutes { get; set; }
    }
}
