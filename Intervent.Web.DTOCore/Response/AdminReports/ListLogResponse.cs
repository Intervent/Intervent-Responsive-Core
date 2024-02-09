namespace Intervent.Web.DTO
{
    public class ListLogResponse
    {
        public IList<ListLogReportDto> report { get; set; }
        public int totalRecords { get; set; }
    }
}
