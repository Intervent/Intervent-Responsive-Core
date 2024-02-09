namespace Intervent.Web.DTO
{
    public class CreateNotificationMessageRequest
    {
        public NotificationMessageDto NotificationMessage { get; set; }

        public bool isSent { get; set; }
    }
}
