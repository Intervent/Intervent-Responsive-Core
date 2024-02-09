namespace Intervent.HWS
{
    public class RegisteredUsersforWebinarsRequest
    {
        public GetWebinarResponse webinarsAPIResponse { get; set; }

        public string webinarId { get; set; }

        public List<int> organizationList { get; set; }
    }
}
