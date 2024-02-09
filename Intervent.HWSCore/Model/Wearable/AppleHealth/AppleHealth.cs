namespace Intervent.HWS.Model
{
    public class AppleHealth
    {
        public BloodPressure bloodPressure { get; set; }
        public DietryEnergy dietryEnergy { get; set; }
        public Water water { get; set; }
        public Fiber fiber { get; set; }
        public Distance distance { get; set; }
        public Steps steps { get; set; }
        public Fat fat { get; set; }
        public Summary summary { get; set; }
        public Sleep sleep { get; set; }
        public CaloriesBurned caloriesBurned { get; set; }
        public Carbohydrate carbohydrate { get; set; }
        public BloodGlucose bloodGlucose { get; set; }
        public Sodium sodium { get; set; }
        public Weight weight { get; set; }
        public Activity activity { get; set; }
        public Protein protein { get; set; }

        public class Activity
        {
            public List<Dataset> dataset { get; set; }
        }

        public class BloodGlucose
        {
            public List<Dataset> dataset { get; set; }
        }

        public class BloodPressure
        {
            public List<Dataset> dataset { get; set; }
        }

        public class CaloriesBurned
        {
            public List<Dataset> dataset { get; set; }
        }

        public class Carbohydrate
        {
            public List<Dataset> dataset { get; set; }
        }

        public class DietryEnergy
        {
            public List<Dataset> dataset { get; set; }
        }

        public class Distance
        {
            public List<Dataset> dataset { get; set; }
        }

        public class Fat
        {
            public List<Dataset> dataset { get; set; }
        }

        public class Fiber
        {
            public List<Dataset> dataset { get; set; }
        }

        public class Protein
        {
            public List<Dataset> dataset { get; set; }
        }

        public class Sleep
        {
            public List<Dataset> dataset { get; set; }
        }

        public class Sodium
        {
            public List<Dataset> dataset { get; set; }
        }

        public class Steps
        {
            public List<Dataset> dataset { get; set; }
        }

        public class Summary
        {
            public List<Dataset> dataset { get; set; }
        }

        public class Water
        {
            public List<Dataset> dataset { get; set; }
        }

        public class Weight
        {
            public List<Dataset> dataset { get; set; }
        }

        public class Dataset
        {
            public DateTime startDateTime { get; set; }
            public DateTime endDateTime { get; set; }
            public string recordId { get; set; }
            public Record record { get; set; }
        }

        public class Record
        {
            //bp
            public int? systolic { get; set; }
            public int? diastolic { get; set; }

            //glucose
            public int? glucose { get; set; }
            public string mealTime { get; set; }
            public int? mealTimeRaw { get; set; }

            //weight
            public double? weight { get; set; }
            public string unit { get; set; }

            //summary
            public int? steps { get; set; }
            public int? caloriesBurned { get; set; }
            public float? distance { get; set; }

            //activity
            public string activityType { get; set; }
            public int? activityTypeRaw { get; set; }
            public float? duration { get; set; }
            public float? totalDistance { get; set; }
            public float? totalEnergyBurned { get; set; }

            //activity summary
            public float? activeEnergyBurned { get; set; }
            public int? appleMoveTime { get; set; }
            public int? appleStandHours { get; set; }
            public int? activityMoveMode { get; set; }
            public int? appleExerciseTime { get; set; }

            //sleep
            public string sleep { get; set; }
            public int? sleepRaw { get; set; }

            //nutricion
            public int? dietryEnergy { get; set; }
            public float? carbohydrate { get; set; }
            public float? fat { get; set; }
            public float? fiber { get; set; }
            public float? protein { get; set; }
            public float? sodium { get; set; }
            public float? water { get; set; }
        }
    }
}
