namespace Intervent.Web.DTO
{
    public sealed class AddEligibilityImportLogRequest
    {
        public EligibilityImportLogDto EligibilityImportLog { get; set; }
    }

    public sealed class BulkAddEligibilityImportLogRequest
    {
        public IEnumerable<EligibilityImportLogDto> EligibilityImportLogs { get; set; }
    }
}
