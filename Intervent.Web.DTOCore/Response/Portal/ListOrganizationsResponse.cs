namespace Intervent.Web.DTO
{
    public class ListOrganizationsResponse
    {
        public IList<OrganizationDto> Organizations { get; set; }

        public int totalRecords { get; set; }
    }
}