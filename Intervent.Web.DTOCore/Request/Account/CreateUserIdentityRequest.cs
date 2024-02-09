namespace Intervent.Web.DTO
{
    public class CreateUserIdentityRequest
    {
        public UserDto user { get; set; }

        public string authType { get; set; }
    }
}