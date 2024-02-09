namespace Intervent.Web.DTO
{
    public class UserKeysDto
    {
        public int Id { get; set; }

        public int userId { get; set; }

        public int portalId { get; set; }

        public string Reason { get; set; }

        public DateTime AcheivedDate { get; set; }

        public UserDto user { get; set; }

        public PortalDto portal { get; set; }
    }
}
