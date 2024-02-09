namespace Intervent.Web.DTO
{
    public class UpdateUserRequest
    {
        public UserDto user { get; set; }

        public bool FromAdmin { get; set; }

        public int UpdatedByUserId { get; set; }

        public bool? IsLocked { get; set; }
    }
}