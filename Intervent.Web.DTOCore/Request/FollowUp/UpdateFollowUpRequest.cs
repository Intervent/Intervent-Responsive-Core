namespace Intervent.Web.DTO
{
    public class UpdateFollowUpRequest
    {
        public PageName pageName { get; set; }

        public int followUpId { get; set; }

        public int userId { get; set; }

        public int portalId { get; set; }

        public bool isIntuityUser { get; set; }

        public string uniqueId { get; set; }

        public int updatedBy { get; set; }

        public string OrganizationCode { get; set; }
    }
}
