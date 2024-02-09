namespace Intervent.Web.DTO
{
    public class AddOrEditNotificationEventRequest
    {
        public NotificationEventDto NotificationEvent { get; set; }

        public bool isSent { get; set; }
    }


    public class BulkAddNotificationEventRequest
    {
        public IEnumerable<NotificationEventDto> NotificationEvents { get; set; }
    }
}
