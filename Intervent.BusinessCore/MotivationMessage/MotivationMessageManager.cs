using Intervent.Business.EmailTriggers;
using Intervent.Web.DataLayer;
using NLog;

namespace Intervent.Business
{
    public class MotivationMessageManager : BaseManager
    {
        public int ProcessMotivationMessage()
        {
            int count = 0;
            try
            {
                MotivationMessageReader reader = new MotivationMessageReader();
                AccountReader _accountReader = new AccountReader();
                var assignedMessages = reader.FetchAssignedMotivationMessages();

                if (assignedMessages != null && assignedMessages.Count > 0)
                {
                    foreach (var assignedMessage in assignedMessages)
                    {
                        var MsgBody = assignedMessage.MotivationMessage.MessageContent;
                        var MsgSubject = assignedMessage.MotivationMessage.Subject;
                        var OrgId = assignedMessage.OrganizationID;
                        var MsgId = assignedMessage.MessagesID;
                        var MsgType = assignedMessage.MessageType;
                        var AssignMsgId = assignedMessage.Id;
                        var MsgTypeId = MsgType.Split(',');
                        for (int i = 0; i < MsgTypeId.Length; i++)
                        {
                            int msgTypeVal = Convert.ToInt16(MsgTypeId[i]);
                            if (msgTypeVal == 1)
                            {
                                //Send email
                                count = count + new EmailTriggerManager().ProcessEmailForMotivationMessage(assignedMessage);
                            }
                            else if (msgTypeVal == 2)
                            {
                                //Send SMS
                                count = count + new TwilioManager().SendMotivationMessageSms(OrgId, MsgBody);
                            }
                            else if (msgTypeVal == 3)
                            {
                                //Update DashboardMessage
                                count = count + reader.AddMotivationMessageToUserDashbaord(MsgBody, OrgId);
                            }
                            reader.MarkMessageComplete(AssignMsgId);
                        }
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
