namespace Intervent.Web.DTO
{
    public class AddEditRoleRequest
    {
        public RoleDto role { get; set; }

        public string adminModules { get; set; }

        public string organizations { get; set; }
    }
}
