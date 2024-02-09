namespace Intervent.Web.DTO
{
    public class GetRoleResponse
    {
        public string RoleName { get; set; }

        public int RoleId { get; set; }

        public int OrganizationId { get; set; }

        public List<int> PortalId { get; set; }
    }
}