namespace Intervent.Web.DTO
{
    public class ListPortalsRequest
    {
        public int? organizationId { get; set; }

        public IEnumerable<int> PortalIds { get; set; }

        public bool? onlyActive { get; set; }
    }
}