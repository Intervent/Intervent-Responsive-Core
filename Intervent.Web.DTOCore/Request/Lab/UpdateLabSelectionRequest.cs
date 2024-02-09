namespace Intervent.Web.DTO
{
    public class UpdateLabSelectionRequest
    {
        public int? Id { get; set; }

        public int UserId { get; set; }

        public int PortalId { get; set; }

        public byte Selection { get; set; }

        public string LabOrderNumber { get; set; }

        public int UpdatedBy { get; set; }

        public string LanguageCode { get; set; }

        public int? DiagnosticLabId { get; set; }

        public bool IntegratedWithLMC { get; set; }

        public byte switchCount { get; set; }

        public bool additionalLab { get; set; }
    }
}
