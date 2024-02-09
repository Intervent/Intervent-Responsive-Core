using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;

namespace Intervent.Business.Notification
{
    public class NotificationManager : BaseManager, INotificationManager
    {
        NotificationReader _NotificationReader;
        AccountReader _AccountReader;
        PortalReader _PortalReader;

        public NotificationManager()
        {
            _NotificationReader = new NotificationReader();
            _AccountReader = new AccountReader();
            _PortalReader = new PortalReader();
        }

        public AddOrEditNotificationEventResponse AddOrEditNotificationEvent(AddOrEditNotificationEventRequest notificationEvent)
        {
            if (notificationEvent.NotificationEvent.Id == null)//add request
            {
                if (notificationEvent.NotificationEvent.NotificationEventTypeId > 0 && notificationEvent.NotificationEvent.NotificationTemplateId <= 0)
                {
                    var isRegistered = true;
                    if (notificationEvent.NotificationEvent.NotificationEventTypeId != NotificationEventTypeDto.POGOStarted.Id &&
                        notificationEvent.NotificationEvent.NotificationEventTypeId != NotificationEventTypeDto.MissedOutreach.Id &&
                        notificationEvent.NotificationEvent.NotificationEventTypeId != NotificationEventTypeDto.POGOTerminated.Id)
                        isRegistered = IsUserRegisteredForNotificationEvent(notificationEvent.NotificationEvent.UserId, notificationEvent.NotificationEvent.NotificationEventTypeId) && !IsUserUnsubscribedForNotificationEvent(notificationEvent.NotificationEvent.UserId, notificationEvent.NotificationEvent.NotificationEventTypeId);
                    if (isRegistered)
                    {
                        var template = GetNotificationTemplate(new GetNotificationTemplateRequest() { NotificationEventTypeId = notificationEvent.NotificationEvent.NotificationEventTypeId }).NotificationTemplate;
                        if (notificationEvent.isSent)
                            notificationEvent.NotificationEvent.NotificationStatusId = NotificationStatusDto.Sent.Id;
                        else
                            notificationEvent.NotificationEvent.NotificationStatusId = NotificationStatusDto.Queued.Id;
                        notificationEvent.NotificationEvent.NotificationTemplateId = template.Id;
                        notificationEvent.NotificationEvent.Subject = notificationEvent.NotificationEvent.Subject != null ? notificationEvent.NotificationEvent.Subject : template.EmailSubject;
                        notificationEvent.NotificationEvent.FromEmailAddress = template.EmailFrom;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return _NotificationReader.AddOrEditNotificationEvent(notificationEvent);
        }

        public void BulkAddNotificationEvent(BulkAddNotificationEventRequest notificationEvents)
        {
            foreach (var notificationEvent in notificationEvents.NotificationEvents)
            {
                if (notificationEvent.Id == null)//add request
                {
                    if (notificationEvent.NotificationEventTypeId > 0 && notificationEvent.NotificationTemplateId <= 0)
                    {
                        if (IsUserRegisteredForNotificationEvent(notificationEvent.To, notificationEvent.NotificationEventTypeId))
                        {
                            var template = GetNotificationTemplate(new GetNotificationTemplateRequest() { NotificationEventTypeId = notificationEvent.NotificationEventTypeId }).NotificationTemplate;
                            notificationEvent.NotificationStatusId = NotificationStatusDto.Queued.Id;
                            notificationEvent.NotificationTemplateId = template.Id;
                            notificationEvent.Subject = template.EmailSubject;
                            notificationEvent.FromEmailAddress = template.EmailFrom;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            _NotificationReader.BulkAddNotificationEvent(notificationEvents);
        }

        public GetNotificationTemplateResponse GetNotificationTemplate(GetNotificationTemplateRequest request)
        {
            return this._NotificationReader.GetNotificationTemplate(request);
        }

        public CreateNotificationMessageResponse CreateNewNotificationMessage(CreateNotificationMessageRequest request)
        {
            //create a new message
            var response = this._NotificationReader.CreateNewNotificationMessage(request);
            //send the email
            if (!request.isSent)
                MailUtility.SendEmail(request.NotificationMessage);
            return response;
        }

        public GetNotificationEventResponse GetNotificationEvent(GetNotificationEventRequest request)
        {
            return _NotificationReader.GetNotificationEvent(request);
        }

        public GetNotificationEventByUniqueIdResponse GetNotificationEventByUniqueId(GetNotificationEventRequest request)
        {
            return _NotificationReader.GetNotificationEventByUniqueId(request);
        }

        public ListNotificationEventResponse ListQueuedNotificationEvents()
        {
            ListNotificationEventRequest request = new ListNotificationEventRequest();
            request.NotificationStatusIds = new int[] { 1 };
            return _NotificationReader.ListNotificationEvents(request);
        }

        public void ProcessQueuedEvents()
        {
            try
            {
                string webSiteUrl;// = NotificationUtil.WebsiteUrl(env);
                var organizations = _PortalReader.ListOrganizations(new ListOrganizationsRequest()).Organizations;
                var queuedEvents = ListQueuedNotificationEvents().NotificationEvents;
                foreach (NotificationEventDto evt in queuedEvents)
                {
                    //create new notification message
                    try
                    {
                        var user = _AccountReader.GetUserById(evt.UserId);
                        string languagePreference = "", subject = evt.Subject ?? evt.NotificationTemplate.EmailSubject;
                        if (!string.IsNullOrEmpty(user.LanguagePreference) && user.LanguagePreference != "en-us")
                        {
                            var notificationTemplateTranslations = evt.NotificationTemplate.NotificationTemplateTranslations.Where(x => x.LanguageCode == user.LanguagePreference).FirstOrDefault();
                            if (notificationTemplateTranslations != null)
                            {
                                languagePreference = user.LanguagePreference;
                                subject = notificationTemplateTranslations.Subject;
                            }
                        }
                        NotificationMessageDto dto = new NotificationMessageDto()
                        {
                            BccAddress = evt.BccAddress,
                            CcAddress = evt.CcAddress,
                            FromAddress = evt.FromEmailAddress ?? evt.NotificationTemplate.EmailFrom,
                            NotificationEventId = evt.Id.Value,
                            ToAddress = evt.ToEmailAddress,
                            Subject = subject
                        };

                        string messageBody;
                        XsltRenderer xsltRenderer = new XsltRenderer(languagePreference);
                        if (evt.NotificationTemplate.TemplateRendererId == 2)
                        {
                            messageBody = xsltRenderer.MessageBody(evt);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(languagePreference))
                            {
                                messageBody = evt.NotificationTemplate.TemplateSource;
                            }
                            else
                            {
                                messageBody = evt.NotificationTemplate.NotificationTemplateTranslations.Where(x => x.LanguageCode == user.LanguagePreference).FirstOrDefault().TemplateSource;
                            }
                        }
                        //replace constants in the message body
                        //like domain
                        webSiteUrl = organizations.FirstOrDefault(s => s.Id == evt.OrganizationId).Url;
                        messageBody = messageBody.Replace(NotificationUtil.WebsiteUrlToken, webSiteUrl);

                        dto.MessageBody = messageBody;
                        if (evt.NotificationTemplate.HasAttachment == true)
                            dto.Attachment = evt.Attachment;
                        CreateNewNotificationMessage(new CreateNotificationMessageRequest() { NotificationMessage = dto });
                        evt.NotificationStatusId = NotificationStatusDto.Sent.Id;
                        AddOrEditNotificationEvent(new AddOrEditNotificationEventRequest() { NotificationEvent = evt });
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error occurred while processing evt " + evt.Id.ToString());
                        evt.NotificationStatusId = NotificationStatusDto.UnknownFailure.Id;
                        AddOrEditNotificationEvent(new AddOrEditNotificationEventRequest() { NotificationEvent = evt });
                    }

                }
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
        }

        public ListNotificationTemplatesResponse ListXsltRendererNotificationTemplates()
        {
            return this._NotificationReader.ListNotificationTemplates(new ListNotificationTemplatesRequest() { NotificationTemplateRendererId = 2 });
        }

        public ListNotificationEventTypePortalResponse ListPortalsRegisteredForNotificationEventType(int notificationEventTypeId)
        {
            var result = _NotificationReader.ListNotificationEventTypePortal(new GetNotificationEventTypePortalRequest() { NotificationEventTypeId = notificationEventTypeId });
            return result;
        }

        public bool IsUserRegisteredForNotificationEvent(int userId, int notificationEventTypeId)
        {
            var portals = ListPortalsRegisteredForNotificationEventType(notificationEventTypeId).Portals;
            var user = _AccountReader.GetUserById(userId);
            ListPortalsRequest request = new ListPortalsRequest();
            request.organizationId = user.OrganizationId;
            var currentPortalId = _PortalReader.CurrentPortalIdForOrganization(request).PortalId;
            return (portals.FirstOrDefault(x => x.Id == currentPortalId) != null) ? true : false;
        }

        public bool IsUserUnsubscribedForNotificationEvent(int userId, int notificationEventTypeId)
        {
            var user = _AccountReader.GetUserById(userId);
            if (!string.IsNullOrEmpty(user.UnsubscribedEmail))
            {
                NotificationReader reader = new NotificationReader();
                var eventDetils = reader.ListNotificationEventType().emails.Where(x => x.Id == notificationEventTypeId && x.NotificationCategory.CanUnsubscribe).FirstOrDefault();
                if (eventDetils != null)
                {
                    foreach (string type in user.UnsubscribedEmail.Split(';'))
                    {
                        if (type == eventDetils.NotificationCategory.Id.ToString())
                            return true;
                    }
                }
            }
            return false;
        }
    }
}
