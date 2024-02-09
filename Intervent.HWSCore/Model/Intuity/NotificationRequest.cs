namespace Intervent.HWS
{
    public class NotificationRequest
    {
        public List<ReminderUser> Users { get; set; }

        public string intuityURL { get; set; }

        public string authToken { get; set; }

    }

    public class ReminderUser
    {
        public int user_id { get; set; }
        public int type { get; set; } // Enumerable, //(e.g. "article", ...)
        public string imageUrl { get; set; }
        public string header_text { get; set; }
        public string body_text { get; set; }
    }
}
