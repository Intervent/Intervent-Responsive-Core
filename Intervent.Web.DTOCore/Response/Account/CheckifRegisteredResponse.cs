namespace Intervent.Web.DTO
{
    public class CheckifRegisteredResponse
    {
        public bool recordExist { get; set; }

        public UserDto User { get; set; }
    }
}
