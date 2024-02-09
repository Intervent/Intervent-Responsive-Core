namespace Intervent.Web.DTO
{
    public class KitsinPortalFollowUpDto
    {
        public int PortalFollowUpId { get; set; }

        public int KitId { get; set; }

        public short Order { get; set; }

        public bool Active { get; set; }

        public KitsDto eduKit { get; set; }

        public PortalFollowUpDto PortalFollowUp { get; set; }
    }
}
