namespace Intervent.Web.DTO
{
    public class AppointmentDTO
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string UserTimeZone { get; set; }

        public string UserName { get; set; }

        public DateTime UTCDate { get; set; }

        public string Date { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public int? Type { get; set; }

        public string Comments { get; set; }

        public string TextResponse { get; set; }

        public bool Active { get; set; }

        public int? InActiveReason { get; set; }

        public string InActiveComment { get; set; }

        public int CoachId { get; set; }

        public string CoachName { get; set; }

        public string CoachBio { get; set; }

        public string CoachPic { get; set; }

        public string CoachMeetingId { get; set; }

        public string ParticipantPic { get; set; }

        public string UserProgram { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public byte Minutes { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string PhoneNumber { get; set; }

        public string Company { get; set; }

        public string Language { get; set; }

        public string ApptType { get; set; }

        public string ScheduledBy { get; set; }

        public bool VideoRequired { get; set; }

        public static string FormatAppointmentDateTime(DateTime date)
        {
            return date.ToString("M/d/yyyy h:mm tt");
        }

        public int Order { get; set; }

        public AppointmentFeedbackDto AppointmentFeedback { get; set; }

        public AppointmentTypesDto AppointmentType { get; set; }

        public int? NSHandledBy { get; set; }

        public DateTime? FutureAppointmentDate { get; set; }

        public UserDto User { get; set; }

        public UserDto User1 { get; set; }

        public UserDto User2 { get; set; }

        public UserDto User3 { get; set; }

        public UserDto User4 { get; set; }
    }
}