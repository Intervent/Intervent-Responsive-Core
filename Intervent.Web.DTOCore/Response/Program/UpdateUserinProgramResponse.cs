namespace Intervent.Web.DTO
{
    public class UpdateUserinProgramResponse
    {
        public bool success { get; set; }

        public bool isProgramCompleted { get; set; }

        public UserDto user { get; set; }
    }
}
