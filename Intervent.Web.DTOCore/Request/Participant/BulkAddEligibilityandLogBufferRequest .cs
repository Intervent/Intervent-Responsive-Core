namespace Intervent.Web.DTO
{
    public class BulkAddEligibilityandLogBufferResponseRequest
    {
        public List<EligibilityDto> eligibilityBuffer { get; set; }

        public List<EligibilityImportLogDto> eligibilityLogBuffer { get; set; }

        public byte? eligibilityFormat { get; set; }

        public int? portalId { get; set; }
    }
}
