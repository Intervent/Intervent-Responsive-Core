namespace Intervent.Web.DTO
{
    public class TimeTrackerResponse
    {
        public IList<UserTimeTrackerDto> timeList { get; set; }

        public int totalRecords { get; set; }
    }
}
