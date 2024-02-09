namespace Intervent.HWS
{
    public class AddOrEditWearableDeviceRequest
    {
        public int userId { get; set; }

        public string externalUserId { get; set; }

        public int wearableDeviceId { get; set; }

        public bool isActive { get; set; }

        public string token { get; set; }

        public string refreshToken { get; set; }

        public string oauthTokenSecret { get; set; }

        public int? offsetFromUTC { get; set; }

        public string deviceId { get; set; }

        public string scope { get; set; }
    }
}
