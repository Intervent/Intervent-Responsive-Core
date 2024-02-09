namespace Intervent.Web.DTO
{
    public class LabErrorLogResponse
    {
        public IList<LabErrorLogDto> report { get; set; }
        public int totalRecords { get; set; }
    }
}
