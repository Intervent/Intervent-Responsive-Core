namespace Intervent.Web.DTO
{
    public class ListTobaccoLogResponse
    {
        public IList<TobaccoLogDto> tobaccoLogLists { get; set; }

        public IList<TobaccoLogDto> tobaccoLogList { get; set; }

        public int totalRecords { get; set; }

    }
}
