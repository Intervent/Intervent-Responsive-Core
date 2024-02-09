namespace Intervent.Web.DTO
{
    public class UserDrugReactionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Drug { get; set; }
        public string Reaction { get; set; }
    }
}
