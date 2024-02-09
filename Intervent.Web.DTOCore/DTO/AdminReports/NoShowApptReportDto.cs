namespace Intervent.Web.DTO
{
    public class NoShowApptReportDto
    {
        public int? ApptId { get; set; }
        public int? UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Language { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string AppointmentType { get; set; }
        public byte? AppointmentLength { get; set; }
        public string WhoAppointmentWasWith { get; set; }
        public string WhoBookedAppointment { get; set; }
        public string AppointmentComments { get; set; }
        public string NSHandledBy { get; set; }
    }
}
