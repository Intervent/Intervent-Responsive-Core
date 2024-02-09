namespace Intervent.HWS.Model
{
    public class GarminBloodPressure
    {
        public string userId { get; set; }
        public string userAccessToken { get; set; }
        public string summaryId { get; set; }
        public int systolic { get; set; }
        public int diastolic { get; set; }
        public int pulse { get; set; }
        public string sourceType { get; set; }
        public int measurementTimeInSeconds { get; set; }
        public int measurementTimeOffsetInSeconds { get; set; }
    }
    public class GarminBloodPressureRequest
    {
        public List<GarminBloodPressure> bloodPressure { get; set; }
    }
}
