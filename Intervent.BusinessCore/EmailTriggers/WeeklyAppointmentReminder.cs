using Intervent.Web.DTO;

namespace Intervent.Business.EmailTriggers
{
    public class WeeklyAppointmentReminder : BaseEmailTrigger, IEmailTrigger
    {
        public override int NotificationTemplateId
        {
            get { return 7; }
        }


        public override int NotificationEventTypeId
        {
            get { return 7; }
        }

        public override EmailTriggerCondition Trigger
        {
            get { return EmailTriggerCondition.WeeklyAppointmentReminder; }
        }

        public override IEnumerable<EmailTriggerDto> ProcessTrigger()
        {
            if (DateTime.UtcNow.DayOfWeek != DayOfWeek.Monday)
                return Enumerable.Empty<EmailTriggerDto>();
            else
            {
                List<EmailTriggerDto> events = new List<EmailTriggerDto>();
                var users = _emailTriggerReader.ListUsers(new ListUsersEmailTriggerRequest() { NotificationEventTypeId = NotificationEventTypeId, TriggerCondition = Trigger }).Users;

                foreach (UserDto user in users)
                {
                    if (user != null)
                    {

                        EmailTriggerDto dto = new EmailTriggerDto();
                        NotificationXsltDataPacketDto xsltDto = new NotificationXsltDataPacketDto();
                        xsltDto.UserFirstName = user.FirstName;
                        xsltDto.UserLastName = user.LastName;
                        var firstAppointment = user.Appointments.First();
                        if (!String.IsNullOrEmpty(user.TimeZone))
                        {
                            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(user.TimeZone);
                            var userDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(firstAppointment.Date), custTZone);
                            xsltDto.AppointmentDateAndTime = AppointmentDTO.FormatAppointmentDateTime(userDateTime);
                        }
                        else
                            xsltDto.AppointmentDateAndTime = AppointmentDTO.FormatAppointmentDateTime(DateTime.Parse(firstAppointment.Date));
                        xsltDto.CoachName = GetCoachName(firstAppointment.CoachId);
                        dto.DataPacket = DTOUtil.SerializeObjectToXml<NotificationXsltDataPacketDto>(xsltDto);
                        dto.TemplateId = NotificationTemplateId;
                        dto.UserId = user.Id;
                        events.Add(dto);
                    }
                }

                return events;
            }
        }
    }
}
