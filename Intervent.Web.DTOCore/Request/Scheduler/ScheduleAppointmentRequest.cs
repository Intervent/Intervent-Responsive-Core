namespace Intervent.Web.DTO
{
    public class ScheduleAppointmentRequest
    {
        public AppointmentDTO Appointment { get; set; }

        public string TimeZone { get; set; }
    }
}