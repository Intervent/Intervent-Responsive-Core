namespace Intervent.Web.DTO
{
    public class MoveAppointmentsRequest
    {
        public List<AppointmentDTO> Appointments { get; set; }

        public int CoachId { get; set; }

        public string ToCoachIds { get; set; }

        public string TimeZone { get; set; }
    }
}
