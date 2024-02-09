using Intervent.Business.Notification;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using NLog;
using System.Globalization;

namespace InterventWebApp
{
    public class NotificationUtility
    {
        private static Logger log = LogManager.GetCurrentClassLogger();

        public static void CreateAppointmentNotificationEvent(NotificationEventTypeDto evt, int userId, int? coachId, DateTime? appointmentDateTime, byte? minutes, string orgContactEmail, string orgContactNumber, string meetingId, bool videoRequired)
        {
            try
            {
                AccountReader reader = new AccountReader();
                var languagePreference = reader.GetUserById(userId).LanguagePreference;
                NotificationManager _notificationManager = new NotificationManager();
                AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
                NotificationEventDto notifyEventDto = new NotificationEventDto();

                notifyEventDto.NotificationEventTypeId = evt.Id;
                notifyEventDto.UserId = userId;
                GetUserRequest request = new GetUserRequest();
                request.id = userId;
                var user = new AccountReader().ReadUser(request).User;
                notifyEventDto.ToEmailAddress = user.Email;
                NotificationXsltDataPacketDto xsltDto = new NotificationXsltDataPacketDto();
                xsltDto.UserFirstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.FirstName.ToLower());
                xsltDto.UserLastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.LastName.ToLower());
                if (appointmentDateTime.HasValue)
                {
                    var shortTimeZone = new String((CommonUtility.GetTimeZones(user.TimeZoneId).TimeZones[0].TimeZoneId).Where(Char.IsUpper).ToArray());
                    var AttointmentDateTime = appointmentDateTime.Value.ToString("M/d/yyyy 'at' h:mm tt");
                    AttointmentDateTime = AttointmentDateTime.Replace("PM", "p.m.").Replace("AM", "a.m.");
                    xsltDto.AppointmentDateAndTime = shortTimeZone != null ? AttointmentDateTime + "(" + shortTimeZone + ")" : AttointmentDateTime;
                    xsltDto.AppointmentStartDateAndTime = AppointmentDTO.FormatAppointmentDateTime(appointmentDateTime.Value);
                    xsltDto.AppointmentEndDateAndTime = AppointmentDTO.FormatAppointmentDateTime(appointmentDateTime.Value.AddMinutes((double)minutes));

                    //this converts startdate from 12 hrs to 24 hrs
                    DateTime dtstart = Convert.ToDateTime(xsltDto.AppointmentStartDateAndTime);
                    var startDate = dtstart.ToString("dd/MM/yyyy HH:mm:ss");

                    //this converts startdate from 12 hrs to 24 hrs
                    DateTime dtend = Convert.ToDateTime(xsltDto.AppointmentEndDateAndTime);
                    var endDate = dtend.ToString("dd/MM/yyyy HH:mm:ss");

                    //this converts data(s) to html url format
                    var datepart = startDate.Split('/');
                    var timepart = datepart[2].Split(' ');
                    string startTime = Uri.EscapeDataString((timepart[0] + "-" + datepart[1] + "-" + datepart[0] + " " + timepart[1]).ToString());
                    var datepart1 = endDate.Split('/');
                    var timepart1 = datepart1[2].Split(' ');
                    string endTime = Uri.EscapeDataString((timepart1[0] + "-" + datepart1[1] + "-" + datepart1[0] + " " + timepart1[1]).ToString());
                    string title = Uri.EscapeDataString("Appointment with my INTERVENT Coach");
                    string meetingLink = "https://zoom.us/j/" + meetingId;
                    string description = "";
                    string location = Uri.EscapeDataString("Phone Call");
                    if (videoRequired)
                    {
                        description = Uri.EscapeDataString("Appointment Confirmed.\n\nLink to join your coaching call : " + meetingLink);
                        location = Uri.EscapeDataString("Video Call");
                    }
                    string timezone = Uri.EscapeDataString(CommonUtility.GetTimeZones(user.TimeZoneId).TimeZones[0].TimeZoneLocation);

                    string googleCal = string.Format("http://addtocalendar.com/atc/google?utz=330&uln=en-US&vjs=1.5&e[0][date_start]={0}&e[0][date_end]={1}&e[0][timezone]={5}&e[0][title]={2}&e[0][description]={3}&e[0][location]={4}", startTime, endTime, title, description, location, timezone);
                    string yahooCal = string.Format("http://addtocalendar.com/atc/yahoo?utz=330&uln=en-US&vjs=1.5&e[0][date_start]={0}&e[0][date_end]={1}&e[0][timezone]={5}&e[0][title]={2}&e[0][description]={3}&e[0][location]={4}", startTime, endTime, title, description, location, timezone);
                    string iCal = string.Format("http://addtocalendar.com/atc/ical?utz=330&uln=en-US&vjs=1.5&e[0][date_start]={0}&e[0][date_end]={1}&e[0][timezone]={5}&e[0][title]={2}&e[0][description]={3}&e[0][location]={4}", startTime, endTime, title, description, location, timezone);
                    string outlookCal = string.Format("http://addtocalendar.com/atc/outlook?utz=330&uln=en-US&vjs=1.5&e[0][date_start]={0}&e[0][date_end]={1}&e[0][timezone]={5}&e[0][title]={2}&e[0][description]={3}&e[0][location]={4}", startTime, endTime, title, description, location, timezone);
                    string helpLink = "https://www.myintervent.com/pdf/videofaq.pdf";
                    if (!string.IsNullOrEmpty(languagePreference) && languagePreference.Equals("fr"))
                    {
                        helpLink = "https://www.myintervent.com/pdf/videofaq_fr.pdf";
                    }

                    xsltDto.GoogleCalendar = googleCal;
                    xsltDto.YahooCalendar = yahooCal;
                    xsltDto.iCalendar = iCal;
                    xsltDto.OutlookCalendar = outlookCal;
                    xsltDto.OrgContactEmail = orgContactEmail;
                    xsltDto.OrgContactNumber = orgContactNumber;
                    xsltDto.MeetingLink = meetingLink;
                    xsltDto.HelpLink = helpLink;
                    xsltDto.VideoRequired = videoRequired;
                }
                if (coachId.HasValue)
                {
                    GetUserRequest coachrequest = new GetUserRequest();
                    coachrequest.id = coachId;
                    var coachUser = new AccountReader().ReadUser(coachrequest).User;
                    xsltDto.CoachName = coachUser.FirstName + " " + coachUser.LastName;
                    xsltDto.CoachName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(xsltDto.CoachName.ToLower());
                }
                notifyEventDto.DataPacket = DTOUtil.SerializeObjectToXml<NotificationXsltDataPacketDto>(xsltDto);
                notificationRequest.NotificationEvent = notifyEventDto;
                _notificationManager.AddOrEditNotificationEvent(notificationRequest);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static void CreateWelcomeNotificationEvent(NotificationEventTypeDto evt, int userId, string email)
        {
            try
            {
                NotificationManager _notificationManager = new NotificationManager();
                AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
                NotificationEventDto notifyEventDto = new NotificationEventDto();
                notifyEventDto.NotificationEventTypeId = evt.Id;
                notifyEventDto.UserId = userId;
                notifyEventDto.ToEmailAddress = email;
                notificationRequest.NotificationEvent = notifyEventDto;
                _notificationManager.AddOrEditNotificationEvent(notificationRequest);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static void CreateLabNotificationEvent(NotificationEventTypeDto evt, int userId, string attachment, string labOrderNumber)
        {
            try
            {
                NotificationManager _notificationManager = new NotificationManager();
                AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
                NotificationEventDto notifyEventDto = new NotificationEventDto();
                GetUserRequest request = new GetUserRequest();
                request.id = userId;
                var user = new AccountReader().ReadUser(request).User;
                notifyEventDto.NotificationEventTypeId = evt.Id;
                notifyEventDto.UserId = userId;
                notifyEventDto.ToEmailAddress = user.Email;
                if (!string.IsNullOrEmpty(labOrderNumber))
                {
                    NotificationXsltDataPacketDto xsltDto = new NotificationXsltDataPacketDto();
                    xsltDto.LabOrderNumber = labOrderNumber;
                    notifyEventDto.DataPacket = DTOUtil.SerializeObjectToXml<NotificationXsltDataPacketDto>(xsltDto);
                }
                if (!string.IsNullOrEmpty(attachment))
                    notifyEventDto.Attachment = attachment;
                notificationRequest.NotificationEvent = notifyEventDto;
                _notificationManager.AddOrEditNotificationEvent(notificationRequest);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static void CreateMissedAppointmentEvent(CreateMissedApptEventRequest request)
        {
            try
            {
                NotificationManager _notificationManager = new NotificationManager();
                AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
                NotificationEventDto notifyEventDto = new NotificationEventDto();

                notifyEventDto.NotificationEventTypeId = request.evt.Id;
                notifyEventDto.UserId = request.userId;
                //get user details
                GetUserRequest userRequest = new GetUserRequest();
                userRequest.id = request.userId;
                var user = new AccountReader().ReadUser(userRequest).User;
                notifyEventDto.ToEmailAddress = user.Email;
                NotificationXsltDataPacketDto xsltDto = new NotificationXsltDataPacketDto();
                xsltDto.UserFirstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.FirstName.ToLower());
                //get coach details
                GetUserRequest coachrequest = new GetUserRequest();
                coachrequest.id = request.coachId;
                var coachUser = new AccountReader().ReadUser(coachrequest).User;
                xsltDto.CoachName = coachUser.FirstName + " " + coachUser.LastName;
                xsltDto.CoachName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(xsltDto.CoachName.ToLower());
                xsltDto.OrgContactEmail = request.orgContactEmail;
                xsltDto.ContactNumber = request.orgContactNumber;
                notifyEventDto.DataPacket = DTOUtil.SerializeObjectToXml<NotificationXsltDataPacketDto>(xsltDto);

                notificationRequest.NotificationEvent = notifyEventDto;
                _notificationManager.AddOrEditNotificationEvent(notificationRequest);
                if (user.Organization != null && user.Organization.IntegrationWith.HasValue && user.Organization.IntegrationWith.Value == (int)IntegrationPartner.Intuity)
                {

                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static void MissedOutreach(NotificationEventTypeDto evt, string uniqueId, int portalId, string email)
        {
            try
            {
                PortalReader portalReader = new PortalReader();
                NotificationManager _notificationManager = new NotificationManager();
                AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
                NotificationEventDto notifyEventDto = new NotificationEventDto();
                notifyEventDto.NotificationEventTypeId = evt.Id;
                notifyEventDto.UniqueId = uniqueId;
                notifyEventDto.PortalId = portalId;
                notifyEventDto.ToEmailAddress = email;
                var organization = portalReader.ReadPortal(new ReadPortalRequest { portalId = portalId }).portal.Organization;
                NotificationXsltDataPacketDto xsltDto = new NotificationXsltDataPacketDto();
                xsltDto.OrgContactEmail = organization.ContactEmail;
                xsltDto.OrgContactNumber = organization.ContactNumber;
                notifyEventDto.DataPacket = DTOUtil.SerializeObjectToXml<NotificationXsltDataPacketDto>(xsltDto);
                notificationRequest.NotificationEvent = notifyEventDto;
                _notificationManager.AddOrEditNotificationEvent(notificationRequest);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static void CreateChangePasswordNotificationEvent(NotificationEventTypeDto evt, int? userId, string email, bool emailchange = false)
        {
            try
            {
                NotificationManager _notificationManager = new NotificationManager();
                AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
                NotificationEventDto notifyEventDto = new NotificationEventDto();
                UserDto user = new UserDto();
                GetUserRequest request = new GetUserRequest();
                if (userId > 0)
                    request.id = userId;
                else
                    request.userName = email;
                user = new AccountReader().ReadUser(request).User;
                if (user != null)
                {
                    notifyEventDto.NotificationEventTypeId = evt.Id;
                    notifyEventDto.ToEmailAddress = (userId > 0 && !emailchange) ? user.Email : email;
                    notifyEventDto.UserId = userId > 0 ? userId ?? default(int) : user.Id;
                    NotificationXsltDataPacketDto xsltDto = new NotificationXsltDataPacketDto();
                    xsltDto.UserFirstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.FirstName.ToLower());
                    xsltDto.UserLastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.LastName.ToLower());
                    notifyEventDto.DataPacket = DTOUtil.SerializeObjectToXml<NotificationXsltDataPacketDto>(xsltDto);
                    notificationRequest.NotificationEvent = notifyEventDto;
                    _notificationManager.AddOrEditNotificationEvent(notificationRequest);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static void CreateFollowUpMessageEvent(NotificationEventTypeDto evt, UserDto userdata, string orgContactEmail, string orgContactNumber)
        {
            try
            {
                NotificationManager _notificationManager = new NotificationManager();
                AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
                NotificationEventDto notifyEventDto = new NotificationEventDto();
                notifyEventDto.NotificationEventTypeId = evt.Id;
                notifyEventDto.UserId = userdata.Id;
                notifyEventDto.ToEmailAddress = userdata.Email;
                NotificationXsltDataPacketDto xsltDto = new NotificationXsltDataPacketDto();
                xsltDto.OrgContactEmail = orgContactEmail;
                xsltDto.OrgContactNumber = orgContactNumber;
                notifyEventDto.DataPacket = DTOUtil.SerializeObjectToXml<NotificationXsltDataPacketDto>(xsltDto);
                notificationRequest.NotificationEvent = notifyEventDto;
                _notificationManager.AddOrEditNotificationEvent(notificationRequest);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static void CreateLMCProgramEvent(NotificationEventTypeDto evt, int userId)
        {
            try
            {
                NotificationManager _notificationManager = new NotificationManager();
                AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
                NotificationEventDto notifyEventDto = new NotificationEventDto();
                var userdata = new AccountReader().ReadUser(new GetUserRequest { id = userId });
                notifyEventDto.NotificationEventTypeId = evt.Id;
                notifyEventDto.UserId = userdata.User.Id;
                notifyEventDto.ToEmailAddress = userdata.User.Email;
                notificationRequest.NotificationEvent = notifyEventDto;
                _notificationManager.AddOrEditNotificationEvent(notificationRequest);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static void CreateDirectMailEvent(NotificationEventTypeDto evt, string email, string emailTemplate)
        {
            try
            {
                NotificationManager _notificationManager = new NotificationManager();
                AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
                NotificationEventDto notifyEventDto = new NotificationEventDto();
                UserDto user = new UserDto();
                user = new AccountReader().ReadUser(new GetUserRequest { userName = email }).User;
                notifyEventDto.NotificationEventTypeId = evt.Id;
                notifyEventDto.UserId = user.Id;
                notifyEventDto.ToEmailAddress = user.Email;
                notificationRequest.NotificationEvent = notifyEventDto;
                notificationRequest.isSent = true;
                var notificationResponse = _notificationManager.AddOrEditNotificationEvent(notificationRequest);
                if (notificationResponse != null)
                    CreateNotificationMessage(notificationResponse.NotificationEventId, emailTemplate);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static void CreateNewMail(NotificationEventTypeDto evt, int to)
        {
            try
            {
                NotificationManager _notificationManager = new NotificationManager();
                AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
                NotificationEventDto notifyEventDto = new NotificationEventDto();
                var userdata = new AccountReader().ReadUser(new GetUserRequest { id = to });
                notifyEventDto.NotificationEventTypeId = evt.Id;
                notifyEventDto.UserId = userdata.User.Id;
                notifyEventDto.ToEmailAddress = userdata.User.Email;
                notificationRequest.NotificationEvent = notifyEventDto;
                _notificationManager.AddOrEditNotificationEvent(notificationRequest);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static void CreateDirectMail(NotificationEventTypeDto evt, int userId)
        {
            try
            {
                NotificationReader notificationReader = new NotificationReader();
                var userdata = new AccountReader().ReadUser(new GetUserRequest { id = userId });
                var notificationTemplate = notificationReader.GetNotificationTemplate(new GetNotificationTemplateRequest() { NotificationTemplateId = evt.Id }).NotificationTemplate;
                //CommonUtility.SendEmail(userdata.User.Email, notificationTemplate.TemplateSource, notificationTemplate.EmailSubject);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static ListNotificationMessageResponse ListNotificationMessage(int participantId)
        {
            NotificationReader reader = new NotificationReader();
            ListNotificationMessageRequest request = new ListNotificationMessageRequest();
            request.UserId = participantId;
            return reader.ListNotificationMessage(request);
        }

        public static bool hasValidEmail(int userId)
        {
            AccountReader acctReader = new AccountReader();
            var user = acctReader.GetUserById(userId);
            return (!user.Email.Contains("noemail.myintervent.com") && !user.Email.Contains("samlnoemail.com"));
        }

        public static void CreateNotificationMessage(long id, string emailTemplate)
        {
            NotificationManager _notificationManager = new NotificationManager();
            var notificationevent = _notificationManager.GetNotificationEvent(new GetNotificationEventRequest { NotificationEventId = id }).NotificationEvent;
            NotificationMessageDto dto = new NotificationMessageDto()
            {
                BccAddress = notificationevent.BccAddress,
                CcAddress = notificationevent.CcAddress,
                FromAddress = notificationevent.FromEmailAddress ?? notificationevent.NotificationTemplate.EmailFrom,
                NotificationEventId = notificationevent.Id.Value,
                ToAddress = notificationevent.ToEmailAddress,
                Subject = notificationevent.Subject ?? notificationevent.NotificationTemplate.EmailSubject,
                MessageBody = emailTemplate
            };
            _notificationManager.CreateNewNotificationMessage(new CreateNotificationMessageRequest { NotificationMessage = dto, isSent = true });
        }

        public static void CreateKitNotificationEvent(NotificationEventTypeDto evt, int userId, string email, string KitName, string orgContactEmail, string orgContactNumber)
        {
            try
            {
                NotificationManager _notificationManager = new NotificationManager();
                AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
                NotificationEventDto notifyEventDto = new NotificationEventDto();
                notifyEventDto.NotificationEventTypeId = evt.Id;
                notifyEventDto.UserId = userId;
                notifyEventDto.ToEmailAddress = email;
                if (!string.IsNullOrEmpty(KitName))
                {
                    NotificationXsltDataPacketDto xsltDto = new NotificationXsltDataPacketDto();
                    xsltDto.KitName = KitName;
                    xsltDto.OrgContactEmail = orgContactEmail;
                    xsltDto.OrgContactNumber = orgContactNumber;
                    notifyEventDto.DataPacket = DTOUtil.SerializeObjectToXml<NotificationXsltDataPacketDto>(xsltDto);
                }
                notificationRequest.NotificationEvent = notifyEventDto;
                _notificationManager.AddOrEditNotificationEvent(notificationRequest);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static void CreateRejectedLabNotificationEvent(NotificationEventTypeDto evt, int userId, string contactNumber, string reason)
        {
            try
            {
                NotificationManager _notificationManager = new NotificationManager();
                AddOrEditNotificationEventRequest notificationRequest = new AddOrEditNotificationEventRequest();
                NotificationEventDto notifyEventDto = new NotificationEventDto();
                GetUserRequest request = new GetUserRequest();
                request.id = userId;
                var user = new AccountReader().ReadUser(request).User;
                notifyEventDto.NotificationEventTypeId = evt.Id;
                notifyEventDto.UserId = userId;
                notifyEventDto.ToEmailAddress = user.Email;
                if (!string.IsNullOrEmpty(contactNumber))
                {
                    NotificationXsltDataPacketDto xsltDto = new NotificationXsltDataPacketDto();
                    xsltDto.ContactNumber = contactNumber;
                    xsltDto.LabRejectionReason = reason;
                    notifyEventDto.DataPacket = DTOUtil.SerializeObjectToXml<NotificationXsltDataPacketDto>(xsltDto);
                }
                notificationRequest.NotificationEvent = notifyEventDto;
                _notificationManager.AddOrEditNotificationEvent(notificationRequest);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static ListNotificationEventTypeResponse ListNotificationEventType()
        {
            NotificationReader reader = new NotificationReader();
            return reader.ListNotificationEventType();
        }

        public static GetNotificationEventByUniqueIdResponse GetNotificationEventByUniqueId(string uniqueId, int portalId)
        {
            NotificationManager _notificationManager = new NotificationManager();
            return _notificationManager.GetNotificationEventByUniqueId(new GetNotificationEventRequest { UniqueId = uniqueId, PortalId = portalId, NotificationEventTypeId = NotificationEventTypeDto.MissedOutreach.Id });
        }

        public static IList<NotificationCategoryDto> ListNotificationCategory()
        {
            NotificationReader reader = new NotificationReader();
            return reader.ListNotificationCategory();
        }
    }
}