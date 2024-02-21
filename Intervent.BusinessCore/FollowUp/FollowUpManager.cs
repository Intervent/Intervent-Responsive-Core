using Intervent.Business.Notification;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using System.Configuration;

namespace Intervent.Business.FollowUp
{
    public class FollowUpManager : BaseManager
    {
        FollowUpReader reader = new FollowUpReader();

        public int AutomateFollowUp()
        {
            var count = 0;
            try
            {
                List<UserDto> users = reader.TriggerFollowUp(Convert.ToInt32(ConfigurationManager.AppSettings["SystemAdminId"]));
                NotificationManager _notificationManager = new NotificationManager();
                count = users.Count();
                foreach (var user in users)
                {
                    if (!user.Email.Contains("noemail.myintervent.com") && !user.Email.Contains("samlnoemail.com"))
                    {
                        AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
                        NotificationEventDto notifyEventDto = new NotificationEventDto();
                        notifyEventDto.NotificationEventTypeId = NotificationEventTypeDto.FollowUp.Id;
                        notifyEventDto.ToEmailAddress = user.Email;
                        notifyEventDto.UserId = user.Id;
                        NotificationXsltDataPacketDto xsltDto = new NotificationXsltDataPacketDto();
                        xsltDto.OrgContactEmail = user.Organization.ContactEmail;
                        xsltDto.OrgContactNumber = user.Organization.ContactNumber;
                        notifyEventDto.DataPacket = DTOUtil.SerializeObjectToXml<NotificationXsltDataPacketDto>(xsltDto);
                        notificationRequest.NotificationEvent = notifyEventDto;
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
