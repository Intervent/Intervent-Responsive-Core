namespace Intervent.Web.DTO
{
    public class NoShowApptReportResponse
    {
        public IList<AppointmentDTO> report { get; set; }

        public int totalRecords { get; set; }
    }
}
