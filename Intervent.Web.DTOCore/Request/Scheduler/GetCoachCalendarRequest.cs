namespace Intervent.Web.DTO
{
    public class GetCoachCalendarRequest
    {
        public int CoachId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int LocRef { get; set; }

        public string TimeZoneId { get; set; }
    }
}
