namespace Intervent.Web.DTO
{
    public class GetNotificationEventRequest
    {
        public long NotificationEventId { get; set; }

        public string UniqueId { get; set; }

        public int PortalId { get; set; }

        public int? NotificationEventTypeId { get; set; }
    }
}
