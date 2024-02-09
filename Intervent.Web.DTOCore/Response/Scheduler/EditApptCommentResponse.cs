namespace Intervent.Web.DTO
{
    public class EditApptCommentResponse
    {
        public bool success { get; set; }

        public bool changeinAppointment { get; set; }

        public AppointmentDTO appointment { get; set; }
    }
}
