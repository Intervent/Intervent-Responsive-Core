namespace Intervent.Web.DTO
{
    public class RoleDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public IList<UserDto> Users { get; set; }

        public IList<AdminModuleDto> AdminModules { get; set; }

        public IList<OrganizationDto> Organizations { get; set; }
    }
}