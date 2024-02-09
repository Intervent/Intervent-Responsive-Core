using Intervent.Web.DTO;

namespace Intervent.Business.Notification
{
    public class PassthroughRenderer : ITemplateRenderer
    {
        public string MessageBody(NotificationEventDto notificationEvent)
        {
            return notificationEvent.NotificationTemplate.TemplateSource;
        }
    }
}
