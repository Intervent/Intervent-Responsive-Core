namespace Intervent.Web.DTO
{
    public class GetUsersByRoleResponse
    {
        public IList<UserDto> users { get; set; }
        public int totalRecords { get; set; }
    }
}