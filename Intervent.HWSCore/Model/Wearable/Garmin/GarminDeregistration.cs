namespace Intervent.HWS.Model.Wearable.Garmin
{
    public class Deregistration
    {
        public string userId { get; set; }
        public string userAccessToken { get; set; }
    }

    public class GarminDeregistrationUsers
    {
        public List<Deregistration> deregistrations { get; set; }
    }

}
