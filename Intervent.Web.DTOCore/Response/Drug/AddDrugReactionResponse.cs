namespace Intervent.Web.DTO
{
    public class AddDrugReactionResponse
    {
        public UserDrugReactionDto drugReaction { get; set; }
        public bool Succeeded { get; set; }
    }
}
