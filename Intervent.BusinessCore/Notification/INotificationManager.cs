using Intervent.Web.DTO;

namespace Intervent.Business.Notification
{
    public interface INotificationManager
    {
        AddOrEditNotificationEventResponse AddOrEditNotificationEvent(AddOrEditNotificationEventRequest notificationEvent);

        GetNotificationEventResponse GetNotificationEvent(GetNotificationEventRequest request);

        GetNotificationTemplateResponse GetNotificationTemplate(GetNotificationTemplateRequest request);

        CreateNotificationMessageResponse CreateNewNotificationMessage(CreateNotificationMessageRequest request);

        ListNotificationEventResponse ListQueuedNotificationEvents();

        void ProcessQueuedEvents();

        ListNotificationTemplatesResponse ListXsltRendererNotificationTemplates();

        ListNotificationEventTypePortalResponse ListPortalsRegisteredForNotificationEventType(int notificationEventTypeId);

        bool IsUserRegisteredForNotificationEvent(int userId, int notificationEventTypeId);

    }
}
