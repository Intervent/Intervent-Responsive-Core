namespace Intervent.HWS.Model
{
    public class OmronMeasurement : ProcessResponse
    {
        public int status { get; set; }

        public Result result { get; set; }

        public class PaginationKey
        {
            public long weightPaginationKey { get; set; }
        }

        public class Result
        {
            public List<Weight> weight { get; set; }

            public List<Activity> activity { get; set; }

            public List<BloodPressure> bloodPressure { get; set; }

            public PaginationKey paginationKey { get; set; }

            public bool truncated { get; set; }

            public int measurementCount { get; set; }
        }

        public class Weight
        {
            public long id { get; set; }

            public long idExt { get; set; }

            public DateTime dateTime { get; set; }

            public DateTime dateTimeLocal { get; set; }

            public string dateTimeUtcOffset { get; set; }

            public string weight { get; set; }

            public string restingMetabolism { get; set; }

            public string visceralFatLevel { get; set; }

            public string skeletalMusclePercentage { get; set; }

            public string weightPref { get; set; }

            public string bmiValue { get; set; }

            public string deviceType { get; set; }

            public string bodyFatPercentage { get; set; }
        }

        public class BloodPressure
        {
            public long id { get; set; }

            public DateTime dateTime { get; set; }

            public DateTime dateTimeLocal { get; set; }

            public string dateTimeUtcOffset { get; set; }

            public string systolic { get; set; }

            public string diastolic { get; set; }

            public string bloodPressureUnits { get; set; }

            public string pulse { get; set; }

            public string pulseUnits { get; set; }

            public string deviceType { get; set; }
        }

        public class Activity
        {
            public long id { get; set; }

            public DateTime dateTime { get; set; }

            public DateTime dateTimeLocal { get; set; }

            public string dateTimeUtcOffset { get; set; }

            public int steps { get; set; }

            public int aerobicSteps { get; set; }

            public int calories { get; set; }

            public string caloriesUnits { get; set; }

            public string dailyDistance { get; set; }

            public string distanceUnits { get; set; }

            public string deviceType { get; set; }

            public List<HourlyActivityDatum> hourlyActivityData { get; set; }
        }

        public class HourlyActivityDatum
        {
            public string hour { get; set; }

            public int distance { get; set; }

            public int aerobicSteps { get; set; }

            public int calories { get; set; }

            public int steps { get; set; }
        }
    }
}
