namespace Intervent.Web.DTO
{
    public class AddUserToRoleRequest
    {
        public int userId { get; set; }

        public string newRole { get; set; }

        public string currentRole { get; set; }
    }
}