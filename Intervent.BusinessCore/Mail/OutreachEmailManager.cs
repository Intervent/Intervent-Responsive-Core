using Intervent.Business.Notification;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;

namespace Intervent.Business.Mail
{
    public class OutreachEmailManager : BaseManager
    {
        AccountReader reader = new AccountReader();

        public int BirthdayWishes()
        {
            int count = 0;
            try
            {
                IList<UserDto> users = reader.TriggerBirthdayWishes();
                NotificationManager _notificationManager = new NotificationManager();
                count = users.Count;
                foreach (var user in users)
                {
                    if (!user.Email.Contains("noemail.myintervent.com") && !user.Email.Contains("samlnoemail.com"))
                    {
                        AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
                        NotificationEventDto notifyEventDto = new NotificationEventDto();
                        notifyEventDto.NotificationEventTypeId = NotificationEventTypeDto.BirthdayWishes.Id;
                        notifyEventDto.ToEmailAddress = user.Email;
                        notifyEventDto.UserId = user.Id;
                        notificationRequest.NotificationEvent = notifyEventDto;
                        NotificationXsltDataPacketDto xsltDto = new NotificationXsltDataPacketDto();
                        xsltDto.UserFirstName = user.FirstName;
                        notifyEventDto.DataPacket = DTOUtil.SerializeObjectToXml<NotificationXsltDataPacketDto>(xsltDto);
                        _notificationManager.AddOrEditNotificationEvent(notificationRequest);
                    }
                }
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
            return count;
        }

        public void IncompleteHRA()
        {
            try
            {
                IList<UserDto> users = reader.IncompleteHRA();
                NotificationManager _notificationManager = new NotificationManager();
                foreach (var user in users)
                {
                    if (!user.Email.Contains("noemail.myintervent.com"))
                    {
                        AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
                        NotificationEventDto notifyEventDto = new NotificationEventDto();
                        notifyEventDto.NotificationEventTypeId = NotificationEventTypeDto.IncompleteHra.Id;
                        notifyEventDto.ToEmailAddress = user.Email;
                        notifyEventDto.UserId = user.Id;
                        notificationRequest.NotificationEvent = notifyEventDto;
                        NotificationXsltDataPacketDto xsltDto = new NotificationXsltDataPacketDto();
                        xsltDto.ContactNumber = user.Organization.ContactNumber;
                        notifyEventDto.DataPacket = DTOUtil.SerializeObjectToXml<NotificationXsltDataPacketDto>(xsltDto);
                        _notificationManager.AddOrEditNotificationEvent(notificationRequest);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public void IncompleteProfile()
        {
            try
            {
                IList<UserDto> users = reader.IncompleteProfile();
                NotificationManager _notificationManager = new NotificationManager();
                foreach (var user in users)
                {
                    if (!user.Email.Contains("noemail.myintervent.com"))
                    {
                        AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
                        NotificationEventDto notifyEventDto = new NotificationEventDto();
                        notifyEventDto.NotificationEventTypeId = NotificationEventTypeDto.IncompleteProfile.Id;
                        notifyEventDto.ToEmailAddress = user.Email;
                        notifyEventDto.UserId = user.Id;
                        notificationRequest.NotificationEvent = notifyEventDto;
                        _notificationManager.AddOrEditNotificationEvent(notificationRequest);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public int SystemDowntime(string emailContent)
        {
            int count = 0;
            try
            {
                IList<UserDto> users = reader.TriggerDowntimeEmail();
                NotificationManager _notificationManager = new NotificationManager();
                count = users.Count;
                foreach (var user in users)
                {
                    if (!user.Email.Contains("noemail.myintervent.com") && !user.Email.Contains("samlnoemail.com"))
                    {
                        AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
                        NotificationEventDto notifyEventDto = new NotificationEventDto();
                        notifyEventDto.NotificationEventTypeId = NotificationEventTypeDto.SystemDowntime.Id;
                        notifyEventDto.ToEmailAddress = user.Email;
                        notifyEventDto.UserId = user.Id;
                        notificationRequest.NotificationEvent = notifyEventDto;
                        NotificationXsltDataPacketDto xsltDto = new NotificationXsltDataPacketDto();
                        xsltDto.UserFirstName = user.FirstName;
                        xsltDto.Downtime = emailContent;
                        xsltDto.OrgContactEmail = user.Organization.ContactEmail;
                        xsltDto.OrgContactNumber = user.Organization.ContactNumber;
                        notifyEventDto.DataPacket = DTOUtil.SerializeObjectToXml<NotificationXsltDataPacketDto>(xsltDto);
                        _notificationManager.AddOrEditNotificationEvent(notificationRequest);
                    }
                }
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
            return count;
        }
    }
}
