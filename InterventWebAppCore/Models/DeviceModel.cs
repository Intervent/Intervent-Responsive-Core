namespace InterventWebApp
{
    public class DeviceModel
    {
        public bool isEmpty { get; set; }

        public int physicalActivityCount { get; set; }

        public int weightCount { get; set; }

        public int bpCount { get; set; }

        public int glucCount { get; set; }

        public int nutritionCount { get; set; }

        public int sleepCount { get; set; }

        public int? showGraphFor { get; set; }

        public int connectedDevicesCount { get; set; }

        public bool HasActivePortal { get; set; }
    }

    public class SummariesData
    {
        public string recentDate { get; set; }

        public int? steps { get; set; }

        public int? remainingSteps { get; set; }

        public int? minutes { get; set; }

        public int? remainingMinutes { get; set; }


    }
    public class RoutineData
    {

        public string leastActiveDay { get; set; }

        public double leastActiveDayValue { get; set; }

        public string mostActiveDay { get; set; }

        public double mostActiveDayValue { get; set; }

        public string leastActiveWeekTimenumber { get; set; }

        public string mostActiveWeekTimenumber { get; set; }

        public double weeklyAvgTime { get; set; }

        public double dailyAvgSteps { get; set; }

        public double leastActiveWeekTime { get; set; }

        public double mostActiveWeekTime { get; set; }

        public string rangeText { get; set; }

        public double avgTime { get; set; }

        public double avgSteps { get; set; }

        public double avgCalories { get; set; }

        public int StepsRange1Value { get; set; }

        public int StepsRange1Count { get; set; }

        public int StepsRange2Value { get; set; }

        public int StepsRange2Count { get; set; }

        public int StepsRange3Value { get; set; }

        public int StepsRange3Count { get; set; }

        public int StepsRange4Value { get; set; }

        public int StepsRange4Count { get; set; }

        public int TimeRange1Value { get; set; }

        public int TimeRange1Count { get; set; }

        public int TimeRange2Value { get; set; }

        public int TimeRange2Count { get; set; }

        public int TimeRange3Value { get; set; }

        public int TimeRange3Count { get; set; }

        public int TimeRange4Value { get; set; }

        public int TimeRange4Count { get; set; }

        public int StepsCount { get; set; }

        public int TimeCount { get; set; }

        public double avgDaily { get; set; }

        public double lowIntensity { get; set; }

        public double moderateIntensity { get; set; }

        public double vigorousIntensity { get; set; }

        public List<GraphData2> graphData { get; set; }

        public List<GraphData2> graphData1 { get; set; }
    }

    public class WeightData
    {
        public int diffDays;

        public string Name { get; set; }

        public int Value { get; set; }

        public double recentBMI { get; set; }

        public double StartingWeight { get; set; }

        public double weightDiffRecent { get; set; }

        public double weightDiffHistory { get; set; }

        public double latestWeight { get; set; }

        public int Count { get; set; }

        public string recentDate { get; set; }

        public double recentWeight { get; set; }

        public double avgWeight { get; set; }

        public double avgBMI { get; set; }

        public double startingBMI { get; set; }

        public double overallBMIdiff { get; set; }

        public List<GraphData1> graphdata { get; set; }
    }

    public class BiometricsData
    {
        public int SystolicValue { get; set; }

        public double SystolicMin { get; set; }

        public double SystolicMax { get; set; }

        public int SystolicRange1Value { get; set; }

        public int SystolicRange1Count { get; set; }

        public int SystolicRange2Value { get; set; }

        public int SystolicRange2Count { get; set; }

        public int SystolicRange3Value { get; set; }

        public int SystolicRange3Count { get; set; }

        public double? LatestSystolic { get; set; }

        public int SystolicCount { get; set; }

        public int DiastolicValue { get; set; }

        public double DiastolicMin { get; set; }

        public double DiastolicMax { get; set; }

        public int DiastolicRange1Value { get; set; }

        public int DiastolicRange1Count { get; set; }

        public int DiastolicRange2Value { get; set; }

        public int DiastolicRange2Count { get; set; }

        public int DiastolicRange3Value { get; set; }

        public int DiastolicRange3Count { get; set; }

        public double? LatestDiastolic { get; set; }

        public int DiastolicCount { get; set; }

        public string recentDate { get; set; }

        public List<GraphData3> graphData { get; set; }

        public List<GraphData1> systolicGraph { get; set; }

        public List<GraphData1> diastolicGraph { get; set; }

        public string DateFormat { get; set; }
    }

    public class SleepData
    {
        public string mostRecent { get; set; }

        public string totalSleep { get; set; }

        public string sleepText { get; set; }

        public string avgSleep { get; set; }

        public string avgDeepSleep { get; set; }

        public string avgTimetoBed { get; set; }

        public string avgTimetoWake { get; set; }

        public string avgLightSleep { get; set; }

        public string avgTimeAwake { get; set; }

        public List<SleepGraphData> weekData { get; set; }

        public List<GraphData1> monthData { get; set; }
    }

    public class SleepGraphData
    {
        public string category { get; set; }

        public string start { get; set; }

        public string end { get; set; }

        public string color { get; set; }
    }

    public class NutritionData
    {
        public string recentDate { get; set; }

        public string fatGoal { get; set; }

        public List<NutritionModel> data { get; set; }

        public int totalRecords { get; set; }

        public double avgCalories { get; set; }

        public string carbGoal { get; set; }

        public string sodiumGoal { get; set; }

        public string avgFat { get; set; }

        public string avgSugars { get; set; }

        public string avgCarb { get; set; }

        public string avgSodium { get; set; }

        public byte? gender { get; set; }
    }

    public class NutritionModel
    {
        public string calories { get; set; }

        public string carbohydrates { get; set; }

        public string fat { get; set; }

        public string fiber { get; set; }

        public string meal { get; set; }

        public string protein { get; set; }

        public string sodium { get; set; }

        public string datestamp { get; set; }

        public string timestamp { get; set; }

        public string user_id { get; set; }
    }

    public class GlucoseData
    {
        public String recentDate { get; set; }

        public bool diabetes { get; set; }

        public GlucometerData average { get; set; }

        public GlucometerData premeal { get; set; }

        public GlucometerData postmeal { get; set; }

        public List<GraphData6> graphData { get; set; }
    }

    public class GraphData1
    {
        public string date { get; set; }

        public double value { get; set; }

        public string label { get; set; }

    }

    public class GraphData2
    {
        public string date { get; set; }

        public int value { get; set; }

        public int value2 { get; set; }
    }

    public class GraphData3
    {
        public string date { get; set; }

        public double open { get; set; }

        public double close { get; set; }
    }

    public class GraphData4
    {
        public string year { get; set; }

        public string income { get; set; }

        public string color { get; set; }
    }


    public class GraphData5
    {
        public string country { get; set; }

        public string litres { get; set; }

    }

    public class GraphData6
    {
        public string x { get; set; }
        public double? ay { get; set; }
        public double? by { get; set; }
        public double? cy { get; set; }
        public string aValue { get; set; }
        public string bValue { get; set; }
        public string cValue { get; set; }
    }
}


