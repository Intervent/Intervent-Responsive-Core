namespace Intervent.Web.DTO
{
    public class ClaimsErrorLogResponse
    {
        public IList<EligibilityImportLogDto> report { get; set; }
        public int totalRecords { get; set; }
    }
}
