namespace Intervent.Web.DTO
{
    public class RegisteredUsersforWebinarDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int WebinarId { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public string UserJoinUrl { get; set; }

        public virtual UserDto User { get; set; }

    }
}
