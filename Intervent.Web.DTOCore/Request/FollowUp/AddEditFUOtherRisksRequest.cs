namespace Intervent.Web.DTO
{
    public class AddEditFUOtherRisksRequest
    {
        public FollowUp_OtherRiskFactorsDto otherRiskFactors { get; set; }

        public int UserId { get; set; }

        public int PortalId { get; set; }

        public int ParticipantId { get; set; }

        public bool IsIntuityUser { get; set; }

        public string UniqueId { get; set; }

        public string OrganizationCode { get; set; }
    }
}