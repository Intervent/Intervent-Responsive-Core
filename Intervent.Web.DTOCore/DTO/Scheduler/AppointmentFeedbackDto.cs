namespace Intervent.Web.DTO
{
    public class AppointmentFeedbackDto
    {
        public int AppointmentId { get; set; }

        public int? Rating { get; set; }

        public string Comments { get; set; }

        public virtual AppointmentDTO Appointment { get; set; }
    }
}
