namespace Intervent.Web.DTO
{
    public class AccessTokensDto
    {
        public int Id { get; set; }

        public string OrgName { get; set; }

        public string Name { get; set; }

        public string Token { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }
}