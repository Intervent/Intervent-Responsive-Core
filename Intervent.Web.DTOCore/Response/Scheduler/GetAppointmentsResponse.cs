namespace Intervent.Web.DTO
{
    public class GetAppointmentsResponse
    {
        public List<AppointmentDTO> Appointments { get; set; }

        public int totalRecords { get; set; }
    }
    public class AppointmentMoveResponse
    {
        public List<AppointmentDTO> AssignedList = new List<AppointmentDTO>();
        public List<AppointmentDTO> NotAssignedList = new List<AppointmentDTO>();
    }
}