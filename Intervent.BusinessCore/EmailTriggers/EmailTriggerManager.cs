using Intervent.Business.Notification;
using Intervent.DAL;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using System.Configuration;

namespace Intervent.Business.EmailTriggers
{
    public class EmailTriggerManager : BaseManager, IEmailTriggerManager
    {
        NotificationManager _NotificationManager;
        AccountReader _AccountReader;
        CommonReader commonReader = new CommonReader();
        private static string EmailUrl = ConfigurationManager.AppSettings["EmailUrl"];

        public EmailTriggerManager()
        {
            _NotificationManager = new NotificationManager();
            _AccountReader = new AccountReader();
        }

        public void ProcessEmailTriggers()
        {
            try
            {
                var triggers = typeof(BaseEmailTrigger)
                                .Assembly.GetTypes()
                                .Where(t => t.IsSubclassOf(typeof(BaseEmailTrigger)) && !t.IsAbstract)
                                .Select(t => (BaseEmailTrigger)Activator.CreateInstance(t));
                List<NotificationEventDto> eventList = new List<NotificationEventDto>();
                foreach (var trigger in triggers)
                {
                    var events = trigger.ProcessTrigger();
                    if (events != null)
                    {
                        foreach (EmailTriggerDto evt in events)
                        {
                            eventList.Add(MapNotificationEvent(evt));
                        }
                        _NotificationManager.BulkAddNotificationEvent(new BulkAddNotificationEventRequest() { NotificationEvents = eventList });
                        eventList.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        NotificationEventDto MapNotificationEvent(EmailTriggerDto source)
        {
            if (source == null)
                return null;
            NotificationEventDto target = new NotificationEventDto();
            target.DataPacket = source.DataPacket;
            target.NotificationTemplateId = source.TemplateId;
            target.UserId = source.UserId;
            target.NotificationStatusId = NotificationStatusDto.Queued.Id;
            //retrieve template details from DB
            var template = this._NotificationManager.GetNotificationTemplate(new GetNotificationTemplateRequest() { NotificationTemplateId = source.TemplateId }).NotificationTemplate;
            target.Subject = template.EmailSubject;
            target.FromEmailAddress = template.EmailFrom;
            target.NotificationEventTypeId = template.NotificationEventTypeId;
            var user = this._AccountReader.GetUser(new GetUserRequest() { id = source.UserId }).Result.User;
            target.ToEmailAddress = user.Email;
            return target;
        }

        public void ProcessEmailForSelfHelpKit()
        {
            EmailTriggerReader reader = new EmailTriggerReader();
            LogReader logreader = new LogReader();
            try
            {
                int count;

                IList<User> users4 = reader.GetSelfHelpKitCompletedUsers();
                if (users4 != null && users4.Count > 0)
                {
                    count = AddNotificationEvent(users4, NotificationEventTypeDto.KitCompletionMotivation.Id, IncentiveMessageTypes.Motivation);
                    var logEvent4 = new LogEventInfo(LogLevel.Info, "Email Service", null, "1 day after kit is completed. Users count : " + count, null, null);
                    logreader.WriteLogMessage(logEvent4);
                }

                List<int> processedUserIds = new List<int>();
                processedUserIds = users4.Select(x => x.Id).ToList();
                IList<User> users = reader.GetSelfHelpKitIncompletedUsers(-8, processedUserIds);
                if (users != null && users.Count > 0)
                {
                    count = AddNotificationEvent(users, NotificationEventTypeDto.KitCompletionStayOnTrack.Id, IncentiveMessageTypes.KitCompletionStayOnTrack);
                    var logEvent = new LogEventInfo(LogLevel.Info, "Email Service", null, "No new kits completed in 7 days since starting program. Users count : " + count, null, null);
                    logreader.WriteLogMessage(logEvent);
                }

                processedUserIds.AddRange(users.Select(x => x.Id).ToList());
                IList<User> users1 = reader.GetSelfHelpKitIncompletedUsers(-15, processedUserIds);
                if (users1 != null && users1.Count > 0)
                {
                    count = AddNotificationEvent(users1, NotificationEventTypeDto.KitCompletionKnowingIsNotDoing.Id, IncentiveMessageTypes.KitCompletionKnowingIsNotDoing);
                    var logEvent1 = new LogEventInfo(LogLevel.Info, "Email Service", null, "No new kits completed in 14 days since starting program. Users count : " + count, null, null);
                    logreader.WriteLogMessage(logEvent1);
                }

                processedUserIds.AddRange(users1.Select(x => x.Id).ToList());
                IList<User> users2 = reader.GetSelfHelpKitIncompletedUsers(-22, processedUserIds);
                if (users2 != null && users2.Count > 0)
                {
                    count = AddNotificationEvent(users2, NotificationEventTypeDto.KitCompletionRecommitToYourHealth.Id, IncentiveMessageTypes.KitCompletionRecommitToYourHealth);
                    var logEvent2 = new LogEventInfo(LogLevel.Info, "Email Service", null, "No new kits completed in 21 days since starting program. Users count : " + count, null, null);
                    logreader.WriteLogMessage(logEvent2);
                }

                processedUserIds.AddRange(users2.Select(x => x.Id).ToList());   
                IList<User> users3 = reader.GetSelfHelpKitIncompletedUsers(-29, processedUserIds);
                if (users3 != null && users3.Count > 0)
                {
                    count = AddNotificationEvent(users3, NotificationEventTypeDto.KitCompletionDontDelayAnyLonger.Id, IncentiveMessageTypes.KitCompletionDontDelayAnyLonger);
                    var logEvent3 = new LogEventInfo(LogLevel.Info, "Email Service", null, "No new kits completed in 28 days since starting program. Users count : " + count, null, null);
                    logreader.WriteLogMessage(logEvent3);

                }
            }
            catch (Exception ex)
            {
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
        }

        private int AddNotificationEvent(IList<User> users, int notificationEventTypeId, string messageType)
        {
            int count = 0;
            NotificationManager _notificationManager = new NotificationManager();
            foreach (var user in users)
            {
                AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
                NotificationEventDto notifyEventDto = new NotificationEventDto();
                notifyEventDto.NotificationEventTypeId = notificationEventTypeId;
                notifyEventDto.ToEmailAddress = user.Email;
                notifyEventDto.UserId = user.Id;
                NotificationXsltDataPacketDto xsltDto = new NotificationXsltDataPacketDto();
                string token = user.Id + ";" + user.Email;
                xsltDto.UnsubscribeURL = EmailUrl + "/Account/EmailSubscription?token=" + commonReader.Encrypt(token);
                notifyEventDto.DataPacket = DTOUtil.SerializeObjectToXml<NotificationXsltDataPacketDto>(xsltDto);
                notificationRequest.NotificationEvent = notifyEventDto;
                var notificationResponse = _notificationManager.AddOrEditNotificationEvent(notificationRequest);
                if (notificationResponse != null && notificationResponse.NotificationEventId != 0)
                {
                    commonReader.AddDashboardMessage(user.Id, messageType, null, null);
                    count++;
                }
            }
            return count;
        }

        public int ProcessEmailForMotivationMessage(AssignedMotivationMessageDto request)
        {
            int count = 0;
            AccountReader reader = new AccountReader();
            NotificationManager _notificationManager = new NotificationManager();
            var users = reader.FindUsers(new FindUsersRequest() { OrganizationIds = new List<int>() { request.OrganizationID } }).Users.Where(x => !string.IsNullOrEmpty(x.Email) && !x.Email.Contains("noemail.myintervent.com") && !x.Email.Contains("samlnoemail.com")).ToList();
            foreach (var user in users)
            {
                AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
                NotificationEventDto notifyEventDto = new NotificationEventDto();
                notifyEventDto.NotificationEventTypeId = NotificationEventTypeDto.MotivaitonMail.Id;
                notifyEventDto.ToEmailAddress = user.Email;
                notifyEventDto.UserId = user.Id;
                notifyEventDto.Subject = request.MotivationMessage.Subject;
                if (!string.IsNullOrEmpty(request.MotivationMessage.MessageContent))
                {
                    NotificationXsltDataPacketDto xsltDto = new NotificationXsltDataPacketDto();
                    xsltDto.MessageBody = request.MotivationMessage.MessageContent;
                    notifyEventDto.DataPacket = DTOUtil.SerializeObjectToXml<NotificationXsltDataPacketDto>(xsltDto);
                }
                notificationRequest.NotificationEvent = notifyEventDto;
                _notificationManager.AddOrEditNotificationEvent(notificationRequest);
                count++;
            }
            return count;
        }
    }
}
