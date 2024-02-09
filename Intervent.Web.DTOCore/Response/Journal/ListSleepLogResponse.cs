namespace Intervent.Web.DTO
{
    public class ListSleepLogResponse
    {
        public IList<SleepLogDto> sleepLogLists { get; set; }

        public IList<SleepLogDto> sleepLogList { get; set; }

        public int totalRecords { get; set; }
    }
}
