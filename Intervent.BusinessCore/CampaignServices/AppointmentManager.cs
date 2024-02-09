using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Intervent.Business
{
    public class AppointmentManager : BaseManager
    {
        static string authToken = System.Configuration.ConfigurationManager.AppSettings["TwilioAuthToken"];
        static string accountSid = System.Configuration.ConfigurationManager.AppSettings["TwilioAccountSID"];
        static string fromNo = System.Configuration.ConfigurationManager.AppSettings["TwilioFrom"];

        public void SendApptReminder()
        {
            try
            {
                ServiceReader reader = new ServiceReader();
                var CallReminderList = reader.GetCallReminderList().CallReminderList;
                SendtheCallListtoTwilio(CallReminderList);
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "SMSService", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
        }

        public void SendLegacyApptReminder()
        {
            try
            {
                ServiceReader reader = new ServiceReader();
                var CallReminderList = reader.GetLegacyCallReminderList().CallReminderList;
                SendtheCallListtoTwilio(CallReminderList);
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "SMSService", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
        }

        public void SendtheCallListtoTwilio(IList<CallReminderLogDto> CallReminderList)
        {
            if (CallReminderList != null)
            {
                CallReminderList = CallReminderList.Where(x => x.Date > DateTime.Now.AddDays(1).Date && x.Date < DateTime.Now.AddDays(2).Date).ToList();
                foreach (var reminder in CallReminderList)
                {
                    if (string.IsNullOrEmpty(reminder.Language))
                        reminder.Language = "en-us";
                    var message = string.Format(GlobalTranslator.Message("L4603", reminder.Language), reminder.Date.ToString("hh:mm tt"), reminder.TimezoneId, reminder.ClientPhone);
                    TwilioClient.Init(accountSid, authToken);

                    var call = CallResource.Create(
                        machineDetection: "DetectMessageEnd",
                        twiml: new Twilio.Types.Twiml("<Response><Say voice='Polly.Joanna-Neural'>" + message + "</Say></Response>"),
                        to: new Twilio.Types.PhoneNumber(reminder.PhoneNumber),
                        from: new Twilio.Types.PhoneNumber(fromNo)
                    );
                }
            }
        }
    }
}