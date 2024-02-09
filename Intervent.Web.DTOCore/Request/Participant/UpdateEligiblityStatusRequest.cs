namespace Intervent.Web.DTO
{
    public class UpdateEligiblityStatusRequest
    {
        public string UniqueId { get; set; }

        public int CreatedBy { get; set; }

        public int OrganizationId { get; set; }

        public int EligiblityStatus { get; set; }

        public int EligiblityReason { get; set; }

        public bool Status { get; set; }
    }
}
