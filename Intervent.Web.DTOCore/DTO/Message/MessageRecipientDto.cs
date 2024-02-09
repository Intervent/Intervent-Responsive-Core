namespace Intervent.Web.DTO
{
    public class MessageRecipientDto
    {
        public int Id { get; set; }

        public int? RecipientId { get; set; }

        public int MessageId { get; set; }

        public bool IsRead { get; set; }

        public int? ReadBy { get; set; }

        public string RecipientName { get; set; }

        public string OwnershipName { get; set; }

        public string OwnershipImage { get; set; }

        public string OwnershipRole { get; set; }

        public int OwnershipId { get; set; }

        public MessageDto Message { get; set; }

        public UserDto User { get; set; }
    }
}
