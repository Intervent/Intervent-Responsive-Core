using Intervent.DAL;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Intervent.Business
{
    public class TwilioManager : BaseManager
    {

        static string twilioAuthToken = System.Configuration.ConfigurationManager.AppSettings["TwilioAuthToken"];
        static string twilioAccountSID = System.Configuration.ConfigurationManager.AppSettings["TwilioAccountSID"];
        static string twilioFrom = System.Configuration.ConfigurationManager.AppSettings["TwilioFrom"];
        static string twilioFromCanada = System.Configuration.ConfigurationManager.AppSettings["TwilioFromCanada"];

        public void SendAppointmentSms()
        {
            try
            {
                Dictionary<int, string> messageSIDs = new Dictionary<int, string>();
                ServiceReader reader = new ServiceReader();
                SchedulerReader schedulerReader = new SchedulerReader();
                ParticipantReader participantReader = new ParticipantReader();

                var reminderList = reader.GetTextMessageCallList();
                if (reminderList != null && reminderList.Count > 0)
                {
                    foreach (var reminder in reminderList)
                    {
                        try
                        {
                            string fromNumber;
                            if (reminder.PortalId == 46)
                                fromNumber = twilioFromCanada;
                            else
                                fromNumber = twilioFrom;
                            TwilioClient.Init(twilioAccountSID, twilioAuthToken);
                            var firstName = reminder.FirstName.Substring(0, 1).ToUpper() + reminder.FirstName.Remove(0, 1);
                            var messageBody = "";
                            if (reminder.Language == "es")
                                messageBody = string.Format("{0}, su cita de entrenamiento es {1}. Responda Y para confirmar, N para reprogramar o llame al {2}. Por favor haga caso omiso si ya ha reprogramado. {3}", firstName, reminder.AptTime.ToString("hh:mm tt"), reminder.ClientPhone, reminder.MeetingLink);
                            else if (reminder.Language == "fr")
                                messageBody = string.Format("{0}, votre rendez-vous de coaching est à {1}. Répondez Y pour confirmer, N pour établir un nouveau rendez-vous ou appelez le {2}. Veuillez ignorer si vous avez déjà reprogrammé votre session. {3}", firstName, reminder.AptTime.ToString("hh:mm tt"), reminder.ClientPhone, reminder.MeetingLink);
                            else
                                messageBody = string.Format("{0}, your coaching appointment is at {1}. Reply Y to confirm, N to reschedule or call {2}. Please disregard if you’ve already rescheduled. {3}", firstName, reminder.AptTime.ToString("hh:mm tt"), reminder.ClientPhone, reminder.MeetingLink);
                            var message = MessageResource.Create(
                                body: messageBody,
                                from: new Twilio.Types.PhoneNumber(fromNumber),
                                to: reminder.PhoneNo
                            );
                            messageSIDs.Add(reminder.AptId, message.To);
                        }
                        catch (Exception ex)
                        {
                            LogReader logReader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Error, "SMSService", null, ex.Message, null, ex);
                            logReader.WriteLogMessage(logEvent);
                            messageSIDs.Add(reminder.AptId, "Not a valid number.");
                            NotesDto noteDto = new NotesDto();
                            noteDto.Admin = SystemAdminId; //System Admin                            
                            noteDto.PortalId = reminder.PortalId;
                            noteDto.userId = reminder.UserId;
                            noteDto.NotesDate = DateTime.MinValue;
                            noteDto.Type = (int)NoteTypes.Other;
                            noteDto.Text = ex.Message;
                            participantReader.AddEditNotes(new AddNotesRequest { note = noteDto, TimeZone = "Eastern Standard Time" });
                        }
                    }
                    schedulerReader.UpdateMessageSID(messageSIDs);
                }

            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "SMSService", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
        }

        public void LegacySendSms()
        {
            try
            {
                Dictionary<int, string> messageSIDs = new Dictionary<int, string>();
                ServiceReader reader = new ServiceReader();
                var reminderList = reader.GetTextMessageCallListFromLegacySystem();
                if (reminderList != null && reminderList.Count > 0)
                {
                    foreach (var reminder in reminderList)
                    {
                        try
                        {
                            TwilioClient.Init(twilioAccountSID, twilioAuthToken);
                            var firstName = reminder.FirstName.Substring(0, 1).ToUpper() + reminder.FirstName.Remove(0, 1);
                            var messageBody = "";
                            if (reminder.CurrLangCode == "FRE")
                                messageBody = string.Format("{0}, votre rendez-vous d'entraînement est {1}. Appelez-nous a 855-494-1093 si vous souhaitez de reprogrammer votre rendez-vous.", firstName, reminder.ApptDate.ToString("hh:mm tt"));
                            else if (reminder.CurrLangCode == "ESP")
                                messageBody = string.Format("{0}, su cita de entrenamiento es a las {1}. Por favor llame al 855-494-1093 si desea reprogramarla.", firstName, reminder.ApptDate.ToString("hh:mm tt"));
                            else
                                messageBody = string.Format("{0}, your coaching appointment is at {1}. Please call 855-494-1093 if you would like to reschedule.", firstName, reminder.ApptDate.ToString("hh:mm tt"));
                            var message = MessageResource.Create(
                                body: messageBody,
                                from: new Twilio.Types.PhoneNumber(twilioFrom),
                                to: reminder.PhoneNumber
                            );
                            messageSIDs.Add(reminder.AppRef, message.To);
                        }
                        catch (Exception ex)
                        {
                            LogReader logReader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Error, "LegacySMSService", null, ex.Message, null, ex);
                            logReader.WriteLogMessage(logEvent);
                            messageSIDs.Add(reminder.AppRef, "Not a valid number.");
                        }
                    }
                    reader.UpdateMessageSIDLegacySystem(messageSIDs);
                }

            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "LegacySMSService", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
        }

        public void SendNewKitSms(int userId)
        {
            try
            {
                Dictionary<int, string> messageSIDs = new Dictionary<int, string>();
                ServiceReader reader = new ServiceReader();
                SchedulerReader schedulerReader = new SchedulerReader();
                ParticipantReader participantReader = new ParticipantReader();
                User smsUser = new User();
                smsUser = reader.GetTextMessageCallUser(userId);
                if (smsUser != null)
                {
                    try
                    {
                        string fromNumber;
                        if (smsUser.Organization.Portals.FirstOrDefault().Id == 46)
                            fromNumber = twilioFromCanada;
                        else
                            fromNumber = twilioFrom;
                        TwilioClient.Init(twilioAccountSID, twilioAuthToken);
                        var firstName = smsUser.FirstName.Substring(0, 1).ToUpper() + smsUser.FirstName.Remove(0, 1);
                        if (string.IsNullOrEmpty(smsUser.LanguagePreference))
                            smsUser.LanguagePreference = "en-us";
                        var messageBody = string.Format(GlobalTranslator.Message("L4602", smsUser.LanguagePreference), smsUser.Organization.Url);
                        var message = MessageResource.Create(
                            body: messageBody,
                            from: new Twilio.Types.PhoneNumber(fromNumber),
                            to: smsUser.CellNumber
                        );
                    }
                    catch (Exception ex)
                    {
                        LogReader logReader = new LogReader();
                        var logEvent = new LogEventInfo(LogLevel.Error, "SMSService", null, ex.Message, null, ex);
                        logReader.WriteLogMessage(logEvent);
                    }
                }

            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "SMSService", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
        }

        public int SendMotivationMessageSms(int OrgId, string MsgBody)
        {
            ServiceReader reader = new ServiceReader();
            var userList = reader.GetMotivationMessageUserList(OrgId);
            int count = 0;
            if (userList != null && userList.Count > 0)
            {
                foreach (var user in userList)
                {
                    try
                    {
                        string fromNumber;
                        if (user.Organization.Portals.FirstOrDefault().Id == 46)
                            fromNumber = twilioFromCanada;
                        else
                            fromNumber = twilioFrom;
                        TwilioClient.Init(twilioAccountSID, twilioAuthToken);
                        var firstName = user.FirstName.Substring(0, 1).ToUpper() + user.FirstName.Remove(0, 1);
                        var messageBody = "";
                        messageBody = string.Format("{0}", MsgBody);
                        var message = MessageResource.Create(
                            body: messageBody,
                            from: new Twilio.Types.PhoneNumber(fromNumber),
                            to: user.CellNumber
                        );
                        count = count + 1;
                    }
                    catch (Exception ex)
                    {
                        LogReader logReader = new LogReader();
                        var logEvent = new LogEventInfo(LogLevel.Error, "SMSService_MotivationMessage", null, ex.Message, null, ex);
                        logReader.WriteLogMessage(logEvent);
                    }
                }
            }
            return count;
        }

        public bool SendDeviceVerificationCode(int userId, string sendTo, string code)
        {
            try
            {
                ServiceReader reader = new ServiceReader();
                User smsUser = reader.GetTextMessageCallUser(userId);
                if (smsUser != null)
                {
                    try
                    {
                        string fromNumber;
                        if (smsUser.Organization.Portals.FirstOrDefault().Id == 46)
                            fromNumber = twilioFromCanada;
                        else
                            fromNumber = twilioFrom;
                        TwilioClient.Init(twilioAccountSID, twilioAuthToken);
                        var firstName = smsUser.FirstName.Substring(0, 1).ToUpper() + smsUser.FirstName.Remove(0, 1);
                        var messageBody = string.Format("Dear {0}, \n {1} is your One Time Password (OTP) to verfiy your device details. " +
                            "\nThis OTP is valid for 10 minutes or 1 successfull attempt whichever is earlier. " +
                            "\nPlease note this otp is valid only for this transaction and cannot be used for any other transaction. " +
                            "\n\nPlease do not disclose/share your OTP with anyone for the security resons.", firstName, code);
                        var message = MessageResource.Create(
                            body: messageBody,
                            from: new Twilio.Types.PhoneNumber(fromNumber),
                            to: sendTo
                        );
                        return true;
                    }
                    catch (Exception ex)
                    {
                        LogReader logReader = new LogReader();
                        var logEvent = new LogEventInfo(LogLevel.Error, "SMSService", null, ex.Message, null, ex);
                        logReader.WriteLogMessage(logEvent);
                    }
                }

            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "SMSService", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
            return false;
        }

        public bool SendSms(string cellNumber, string msgBody)
        {
            try
            {
                TwilioClient.Init(twilioAccountSID, twilioAuthToken);
                var messageBody = "";
                messageBody = string.Format("{0}", msgBody);
                var message = MessageResource.Create(
                    body: messageBody,
                    from: new Twilio.Types.PhoneNumber(twilioFrom),
                    to: cellNumber
                );
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "SMSService_SendSms", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }

            return true;
        }
    }
}
