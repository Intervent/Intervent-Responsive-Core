using Intervent.Web.DTO;

namespace Intervent.Business.Notification
{
    public interface ITemplateRenderer
    {
        string MessageBody(NotificationEventDto notificationEvent);
    }
}
