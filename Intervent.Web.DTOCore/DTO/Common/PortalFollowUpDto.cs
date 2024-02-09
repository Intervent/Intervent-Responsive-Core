namespace Intervent.Web.DTO
{
    public class PortalFollowUpDto
    {
        public int Id { get; set; }

        public int FollowupTypeId { get; set; }

        public int PortalId { get; set; }

        public bool LabIntegration { get; set; }

        public byte ProgramType { get; set; }

        public FollowUpTypeDto FollowUpType { get; set; }

        public IList<KitsinPortalFollowUpDto> KitsinPortalFollowUps { get; set; }
    }
}
