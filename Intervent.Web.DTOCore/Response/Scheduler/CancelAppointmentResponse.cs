namespace Intervent.Web.DTO
{
    public class CancelAppointmentResponse
    {
        public bool success { get; set; }

        public int userId { get; set; }

        public int coachId { get; set; }

        public string orgContactNumber { get; set; }

        public string orgContactEmail { get; set; }

        public int portalId { get; set; }

        public byte? integrationWith { get; set; }

        public DateTime AptTime { get; set; }
    }
}
