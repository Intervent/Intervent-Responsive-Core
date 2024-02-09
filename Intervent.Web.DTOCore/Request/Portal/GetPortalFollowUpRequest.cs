namespace Intervent.Web.DTO
{
    public class GetPortalFollowUpRequest
    {
        public int PortalId { get; set; }

        public int? ProgramType { get; set; }

        public bool bothFollowUps { get; set; }
    }
}
