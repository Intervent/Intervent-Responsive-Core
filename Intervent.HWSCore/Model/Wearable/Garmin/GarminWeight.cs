namespace Intervent.HWS.Model.Wearable.Garmin
{
    public class BodyComp
    {
        public string userId { get; set; }
        public string userAccessToken { get; set; }
        public string summaryId { get; set; }
        public int muscleMassInGrams { get; set; }
        public int boneMassInGrams { get; set; }
        public double bodyWaterInPercent { get; set; }
        public double bodyFatInPercent { get; set; }
        public double bodyMassIndex { get; set; }
        public int weightInGrams { get; set; }
        public int measurementTimeInSeconds { get; set; }
        public int measurementTimeOffsetInSeconds { get; set; }
    }
    public class GarminWeightRequest
    {
        public List<BodyComp> bodyComps { get; set; }
    }
}
