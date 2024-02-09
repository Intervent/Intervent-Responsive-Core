namespace Intervent.HWS
{
    public class AddRegisteredUsersforWebinarsRequest
    {
        public int userId { get; set; }

        public int webinarId { get; set; }

        public DateTime registrationDate { get; set; }

        public string joinUrl { get; set; }
    }
}
