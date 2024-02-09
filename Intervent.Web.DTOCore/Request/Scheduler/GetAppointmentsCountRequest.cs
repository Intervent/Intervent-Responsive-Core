namespace Intervent.Web.DTO
{
    public class GetAppointmentsCountRequest
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? coachId { get; set; }

        public string TimeZone { get; set; }
    }
}