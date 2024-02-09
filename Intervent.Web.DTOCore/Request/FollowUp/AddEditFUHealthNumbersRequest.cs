namespace Intervent.Web.DTO
{
    public class AddEditFUHealthNumbersRequest
    {
        public FollowUp_HealthNumbersDto healthNumbersDto { get; set; }

        public int UserId { get; set; }

        public int PortalId { get; set; }

        public int ParticipantId { get; set; }

        public bool bloodwork { get; set; }

        public bool IsIntuityUser { get; set; }

        public string UniqueId { get; set; }

        public string OrganizationCode { get; set; }
    }
}