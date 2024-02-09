namespace Intervent.Web.DTO
{
    public class FavoriteContactDto
    {
        public int Id { get; set; }
        public string Picture { get; set; }
        public string ContactName { get; set; }
        public string RoleName { get; set; }
        public UserDto User { get; set; }
    }
}
