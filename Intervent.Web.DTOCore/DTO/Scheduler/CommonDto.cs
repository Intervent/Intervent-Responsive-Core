namespace Intervent.Web.DTO
{
    public class AvailabilityDto
    {
        public string AvailDate { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string StartTimeString
        {
            get
            {
                return StartTime.ToString();
            }
        }

        public string EndTimeString
        {
            get
            {
                return EndTime.ToString();
            }
        }

        public int CoachId { get; set; }

        public string CoachName { get; set; }

        public bool Disable { get; set; }

        public string RefId { get; set; }

        public bool HasNextAvail { get; set; }

        public string UserRoleCode { get; set; }

    }

    public class CountByDate
    {
        public DateTime Date { get; set; }

        public string DateString
        {
            get
            {
                return Date.ToString();
            }
        }

        public int NoOfRecords { get; set; }
    }
}