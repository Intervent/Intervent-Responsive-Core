namespace Intervent.Web.DTO
{
    public class ListPortalsResponse
    {
        public IList<PortalDto> portals { get; set; }

        public string OrgName { get; set; }

        public string OrgCode { get; set; }
    }
}