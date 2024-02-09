namespace Intervent.Web.DTO
{
    public class ScheduleAppointmentResponse
    {
        public string ConfirmationMessage { get; set; }

        public int Status { get; set; }

        public string ErrorMessage { get; set; }

        public int apptId { get; set; }

        public EnrollinProgramResponse enrollinProgramResponse { get; set; }
    }
}