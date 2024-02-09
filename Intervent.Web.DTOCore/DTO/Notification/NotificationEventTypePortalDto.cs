namespace Intervent.Web.DTO
{
    public class NotificationEventTypePortalDto
    {
        public int NotificationEventTypeId { get; set; }

        public int PortalId { get; set; }

        public string types { get; set; }

        public NotificationEventTypeDto NotificationEventType { get; set; }
    }
}
