namespace Intervent.Web.DTO
{
    public class ListNotificationEventTypePortalResponse
    {
        public IEnumerable<NotificationEventTypeDto> NotificationEventTypePortals { get; set; }

        public IEnumerable<PortalDto> Portals { get; set; }

    }
}
