namespace Intervent.Web.DTO
{
    public class SreeningDataErrorLogResponse
    {
        public IList<ScreeningDataErrorLogDto> report { get; set; }
        public int totalRecords { get; set; }
    }
}
