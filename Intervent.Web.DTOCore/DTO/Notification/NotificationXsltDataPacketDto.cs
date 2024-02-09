using System.Xml.Serialization;

namespace Intervent.Web.DTO
{
    [XmlRootAttribute("Notification", Namespace = "https://www.myintervent.com", IsNullable = false)]
    public class NotificationXsltDataPacketDto
    {
        public string UserFirstName { get; set; }

        public string UserLastName { get; set; }

        public string CoachName { get; set; }

        public string AppointmentDateAndTime { get; set; }

        public string ContactNumber { get; set; }

        public string AppointmentStartDateAndTime { get; set; }

        public string AppointmentEndDateAndTime { get; set; }

        public string GoogleCalendar { get; set; }

        public string YahooCalendar { get; set; }

        public string iCalendar { get; set; }

        public string OutlookCalendar { get; set; }

        public string LabOrderNumber { get; set; }

        public string KitName { get; set; }

        public string LabRejectionReason { get; set; }

        public string OrgContactEmail { get; set; }

        public string OrgContactNumber { get; set; }

        public string Downtime { get; set; }

        public string MeetingLink { get; set; }

        public string HelpLink { get; set; }

        public bool VideoRequired { get; set; }

        public string UnsubscribeURL { get; set; }

        public string MessageBody { get; set; }
    }
}
