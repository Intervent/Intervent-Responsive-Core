namespace Intervent.Web.DTO
{
    public class SetCoachAvailabilityRequest
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string Days { get; set; }

        public int coachId { get; set; }

        public string TimeZone { get; set; }

        public int CreatedBy { get; set; }
    }
}

